using UnityEngine;

namespace RPG.Network.Management.Managers {
    public abstract class BaseManager : MonoBehaviour, IManager {
        public string Token { get; set; }
        public abstract void Disconnect();
    }
}
