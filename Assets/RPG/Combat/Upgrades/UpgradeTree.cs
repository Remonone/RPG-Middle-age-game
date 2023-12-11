using System;
using System.Collections.Generic;
using System.Linq;
using RPG.Combat.Modifiers.BaseTypes;
using RPG.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Combat.Upgrades {
    [CreateAssetMenu(fileName = "Upgrade Tree", menuName = "GumuPeachu/Combat/Upgrades/Upgrade Tree", order = 0)]
    public class UpgradeTree : ScriptableObject, ISerializationCallbackReceiver {
        [ReadOnly] [SerializeField] private string _treeID;
        [SerializeField] private CreatureClass _class;
        [SerializeField] private List<UpgradeNode> _nodes;

        public CreatureClass Class => _class;
        public List<UpgradeNode> Nodes => _nodes;
        
        private static Dictionary<CreatureClass, UpgradeTree> _upgradeTrees;

        public static UpgradeTree GetUpgradeTree(CreatureClass creatureClass) {
            if (ReferenceEquals(_upgradeTrees, null)) LoadStore();
            return _upgradeTrees![creatureClass];
        }
        
        private static void LoadStore() {
            _upgradeTrees = new Dictionary<CreatureClass, UpgradeTree>();
            var trees = Resources.LoadAll<UpgradeTree>("Upgrades");
            foreach (var tree in trees) {
                if (!_upgradeTrees.ContainsKey(tree.Class)) {
                    _upgradeTrees.Add(tree.Class, tree);
                } else {
                    Debug.Log($"Found collision of upgrade trees while loading resources. Trying to create different {tree.Class} tree on existing one.");
                }
            }
        }

        public void OnBeforeSerialize() {
            if (string.IsNullOrWhiteSpace(_treeID)) {
                _treeID = Guid.NewGuid().ToString();
            }
        }
        
        public void OnAfterDeserialize() {}
    }
    [Serializable]
    public sealed class UpgradeNode {
        public Image UpgradeImage;
        public string ID;
        public string Name;
        public int Cost;
        [TextArea] public string Description;
        [SerializeField] private Modification[] _modifications;
        [Tooltip("A list of node ids to open this upgrade")]
        [SerializeField] private string[] _requisites;

        public bool IsUpgradeSatisfied(string[] upgradeList) =>
            _requisites.All(upgradeList.Contains);

        public void Apply(GameObject invoker) {
            foreach (var modification in _modifications) {
                modification.RegisterModification(invoker);
            }
        }

        public void Cancel() {
            foreach (var modification in _modifications) {
                modification.UnregisterModification();
            }
        }
    }
}
