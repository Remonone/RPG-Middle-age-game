using System;
using System.Collections.Generic;
using RPG.Core.Predicate;
using RPG.Inventories;
using RPG.Stats;
using UnityEngine;

namespace RPG.Quests {
    public class QuestStore : MonoBehaviour {
        private List<QuestState> _states = new();
        private PredicateMonoBehaviour _predicate;

        public IEnumerable<QuestState> States => _states;

        public event Action<QuestState> OnStateUpdated;

        private void Awake() {
            _predicate = GetComponent<PredicateMonoBehaviour>();
        }

        public void AddQuest(Quest quest) {
            if (HasQuest(quest)) return;
            var state = new QuestState(this, quest);
            _states.Add(state);
            OnStateUpdated?.Invoke(state);
        }
        
        private void GiveAward(Quest quest) {
            var inventory = GetComponent<Inventory>();
            var dropper = GetComponent<ItemDropper>();
            var baseStats = GetComponent<BaseStats>();
            foreach (var reward in quest.Rewards) {
                var success = inventory.AddToFirstEmptySlot(reward.Item, reward.Count);
                if (!success) {
                    dropper.DropItem(reward.Item, reward.Count);
                }
            }
            baseStats.AddExperience(quest.RewardExperience);
        }

        private void FinishQuest(QuestState state) {
            _states.Remove(state);
            GiveAward(state.Quest);
        }

        private bool HasQuest(Quest quest) => GetQuestState(quest) != null;

        private QuestState GetQuestState(Quest quest) => _states.Find(element => element.Quest.Equals(quest));

        public void UpdateState(QuestState questState) {
            OnStateUpdated?.Invoke(questState);
            if (questState.IsCompleted) {
                FinishQuest(questState);
            }
        }
    }
}
