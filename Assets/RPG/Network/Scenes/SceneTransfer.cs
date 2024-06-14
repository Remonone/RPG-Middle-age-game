using System;
using System.Collections.Generic;
using System.Linq;
using RPG.Movement;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPG.Network.Scenes {
    [RequireComponent(typeof(BoxCollider))]
    public class SceneTransfer : NetworkBehaviour {

        [SerializeField] private string _sceneNameToTransfer;
        [SerializeField] private Vector3 _positionToSpawn;
        
        private List<ulong> _objectsInRange = new ();



        private void OnTriggerEnter(Collider other) {
            if (other.TryGetComponent<NetworkObject>(out var obj)) {
                if(obj.CompareTag("Player")) _objectsInRange.Add(obj.OwnerClientId);
                if (CheckIsAllPlayersInBounds()) {
                    FindObjectOfType<SceneDistributor>().TransferToDifferentSceneWithCoordsServerRpc(_sceneNameToTransfer, _positionToSpawn);
                }
            }
        }

        private bool CheckIsAllPlayersInBounds() {
            return NetworkManager.Singleton.ConnectedClients.Keys.All(id => _objectsInRange.Contains(id));
        }
    }
}