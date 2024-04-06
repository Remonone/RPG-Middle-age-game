using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using RPG.Saving;
using RPG.Stats;
using UnityEngine;

namespace RPG.Combat.Upgrades {
    public class PlayerUpgrade : MonoBehaviour, ISaveable {

        private BaseStats _stats;
        private UpgradeTree _upgradeTree;
        private int _availableUpgradePoints;
        private readonly List<string> _upgradeList = new();

        private void Awake() {
            _stats = GetComponent<BaseStats>();
            _upgradeTree = UpgradeTree.GetUpgradeTree(_stats.CreatureClass);
        }

        private void OnEnable() {
            _stats.OnLevelUp += UpdateSkillPoints;
        }

        private void OnDisable() {
            _stats.OnLevelUp -= UpdateSkillPoints;
        }

        private void UpdateSkillPoints() {
            _availableUpgradePoints += 1;
        }

        private bool UpgradePlayer(string upgradeId) {
            var upgradeItem = _upgradeTree.Nodes.Single(node => node.ID == upgradeId);
            if (_availableUpgradePoints < upgradeItem.Cost) return false;
            if (_upgradeList.Contains(upgradeId)) return false;
            if (!upgradeItem.IsUpgradeSatisfied(_upgradeList.ToArray())) return false;
            upgradeItem.Apply(gameObject);
            _upgradeList.Add(upgradeId);
            _availableUpgradePoints -= upgradeItem.Cost;
            return true;
        }
        
        public JToken CaptureAsJToken() {
            throw new NotImplementedException();
        }
        public void RestoreFromJToken(JToken state) {
            throw new NotImplementedException();
        }
    }
}
