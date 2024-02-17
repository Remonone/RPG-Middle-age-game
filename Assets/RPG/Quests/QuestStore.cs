using System;
using System.Collections.Generic;
using RPG.Core.Predicate;
using RPG.Inventories;
using UnityEngine;

namespace RPG.Quests {
    public class QuestStore : PredicateMonoBehaviour {
        private List<QuestState> _states = new();

        public IEnumerable<QuestState> States => _states;

        public event Action OnStateUpdated;

        public void AddQuest(Quest quest) {
            if (HasQuest(quest)) return;
            var state = new QuestState(quest);
            _states.Add(state);
            OnStateUpdated?.Invoke();
        }

        public void CompleteObjective(Quest quest, string objectiveId) {
            var state = GetQuestState(quest);
            state.CompleteObjective(objectiveId);
            if (state.IsCompleted) {
                GiveAward(quest);
            }
        }
        private void GiveAward(Quest quest) {
            var inventory = GetComponent<Inventory>();
            var dropper = GetComponent<ItemDropper>();
            foreach (var reward in quest.Rewards) {
                var success = inventory.AddToFirstEmptySlot(reward.Item, reward.Count);
                if (!success) {
                    dropper.DropItem(reward.Item, reward.Count);
                }
            }
        }

        private bool HasQuest(Quest quest) => GetQuestState(quest) != null;

        private QuestState GetQuestState(Quest quest) => _states.Find(element => element.Quest.Equals(quest));

        public override object Predicate(string command, object[] arguments) {
            return null;
        }
    }
}
