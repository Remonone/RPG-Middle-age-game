using RPG.Creatures.Player;
using Unity.Netcode;

namespace RPG.Network.Processors {
    public class GameProcessor : NetworkBehaviour {
        private void Start() {
            DontDestroyOnLoad(this);
        }

        public void InitPlayers() {
            foreach (var player in NetworkManager.ConnectedClients.Values) {
                ClientRpcParams param = new ClientRpcParams {
                    Send = {
                        TargetClientIds = new[] { player.ClientId }
                    }
                };
                player.PlayerObject.GetComponent<PlayerController>().LoadClientRpc(param);
            }
        }
    }
}