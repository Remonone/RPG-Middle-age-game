using System;
using Newtonsoft.Json.Linq;
using RPG.Lobby;
using RPG.Network.Management;
using RPG.Saving;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine.SceneManagement;

namespace RPG.Creatures.Player {
    public class NetworkPlayer : NetworkBehaviour {

        public NetworkVariable<PlayerData> _playerInfo;
        private bool _isPlayerInitiated;
        
        private void Awake() {
            NetworkManager.Singleton.GetComponent<UnityTransport>().OnTransportEvent += OnTransportEvent;
            _playerInfo = new NetworkVariable<PlayerData>();
        }

        public override void OnNetworkSpawn() {
            base.OnNetworkSpawn();
            if (!IsOwner) return;
            var data = FindObjectOfType<ApplicationManager>().PlayerData;
            SetPlayerDataServerRpc(data);
        }

        [ServerRpc]
        private void SetPlayerDataServerRpc(PlayerData data) {
            if (_isPlayerInitiated) return;
            _playerInfo.Value = data;
            _isPlayerInitiated = true;
        }

        private void OnTransportEvent(NetworkEvent eventtype, ulong clientid, ArraySegment<byte> payload, float receivetime) {
            if (eventtype == NetworkEvent.Disconnect) {
                SceneManager.LoadScene("Main");
                FindObjectOfType<ApplicationManager>().SwitchPlayerState(ServerState.NONE, OwnerClientId);
            }
        }

        [ClientRpc]
        public void SwitchPlayerStateClientRpc(bool state) {
            gameObject.SetActive(state);
            SwitchPlayerStateServerRpc(state);
        }

        [ServerRpc(RequireOwnership = false)]
        private void SwitchPlayerStateServerRpc(bool state) {
            gameObject.SetActive(state);
        }

        [ServerRpc(RequireOwnership = false)]
        public void DisconnectPlayerFromServerRpc(ServerRpcParams serverRpcParams = default) {
            var playerId = serverRpcParams.Receive.SenderClientId;
            ApplicationManager.Instance.SavePlayerToDatabase(playerId);
            NetworkManager.Singleton.DisconnectClient(playerId);
        }

        [ClientRpc]
        public void InitPlayerClientRpc(string data, ClientRpcParams clientRpcParams = default) {
            gameObject.SetActive(true);
            ApplicationManager.Instance.SwitchPlayerState(ServerState.STARTED, OwnerClientId);
            GetComponent<SaveableEntity>().RestoreFromJToken(JToken.Parse(data));
            GetComponent<PlayerController>().Init();
        }
    }
}