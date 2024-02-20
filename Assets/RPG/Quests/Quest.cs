using System;
using System.Collections.Generic;
using System.Linq;
using RPG.Inventories.Items;
using RPG.Utils;
using UnityEngine;

namespace RPG.Quests {
    [CreateAssetMenu(fileName = "New Quest", menuName = "GumuPeachu/Quest/Create New Quest", order = 0)]
    public class Quest : ScriptableObject {
        [SerializeField] private string _name;
        [SerializeField] private List<Objective> _objectives;
        [SerializeField] private List<Reward> _rewards;
        [SerializeField] private float _rewardExperience;

        public string Title => _name;
        public IEnumerable<Objective> Objectives => _objectives;
        public IEnumerable<Reward> Rewards => _rewards;

        private static Dictionary<string, Quest> _questDictionary;

        public bool HasObjective(string id) {
            return _objectives.Any(objective => objective.Id == id);
        }

        public static Quest GetQuestByName(string questName) {
            _questDictionary ??= GetFilledQuestDictionary();
            return _questDictionary[questName];
        }
        
        private static Dictionary<string, Quest> GetFilledQuestDictionary() {
            var questDictionary = new Dictionary<string, Quest>();
            var quests = Resources.LoadAll<Quest>(PropertyConstants.QUESTS_PATH);
            foreach (var quest in quests) {
                if (questDictionary.ContainsKey(quest._name)) {
                    Debug.LogError($"There's a duplicate for objects: {questDictionary[quest._name]} and {quest}");
                    continue;
                }
                _questDictionary.Add(quest._name, quest);
            }
            return questDictionary;
        }

    }

    [Serializable]
    public class Reward {
        public InventoryItem Item;
        public int Count;
    }
    
    [Serializable]
    public class Objective {
        public string Id;
        public string Description;
        public string Condition;
    }
}
