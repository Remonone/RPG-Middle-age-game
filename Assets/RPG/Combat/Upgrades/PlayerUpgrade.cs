
using RPG.Core;
using UnityEngine;

namespace RPG.Combat.Upgrades {
    public class PlayerUpgrade : MonoBehaviour {

        private CreatureInfo _info;
        private UpgradeTree _upgradeTree;

        private void Awake() {
            _info = GetComponent<CreatureInfo>();
            _upgradeTree = UpgradeTree.GetUpgradeTree(_info.CreatureClass);
        }
    }
}
