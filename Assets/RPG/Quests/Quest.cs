using System;
using System.Collections.Generic;
using System.Linq;
using RPG.Inventories.Items;
using RPG.Quests.Objectives;
using RPG.Utils;
using UnityEngine;

namespace RPG.Quests {
    [CreateAssetMenu(fileName = "New Quest", menuName = "GumuPeachu/Quest/Create New Quest", order = 0)]
    public class Quest : ScriptableObject {
        [SerializeField] private string _name;
        [SerializeField] private List<QuestObjective> _objectives;
        [SerializeField] private List<Reward> _rewards;
        [SerializeField] private float _rewardExperience;

        public string Title => _name;
        public IEnumerable<QuestObjective> Objectives => _objectives;
        public IEnumerable<Reward> Rewards => _rewards;
        public float RewardExperience => _rewardExperience;

        private static Dictionary<string, Quest> _questDictionary;

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
    
}
