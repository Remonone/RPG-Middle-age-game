using Unity.Netcode.Components;

namespace RPG.Network.Client {
    public class ClientNetworkTransform : NetworkTransform {

        public override void OnNetworkSpawn() {
            base.OnNetworkSpawn();
            CanCommitToTransform = IsOwner;
        }

        protected override bool OnIsServerAuthoritative() {
            return false;
        }

        protected override void Update() {
            CanCommitToTransform = IsOwner;
            base.Update();
            if (NetworkManager != null) {
                if (NetworkManager.IsConnectedClient || NetworkManager.IsListening) {
                    if(CanCommitToTransform) TryCommitTransformToServer(transform, NetworkManager.LocalTime.Time);
                }
            }
        }
    }
}
