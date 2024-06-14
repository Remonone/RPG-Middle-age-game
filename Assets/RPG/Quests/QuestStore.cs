using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using RPG.Core.Predicate.Interfaces;
using RPG.Inventories;
using RPG.Saving;
using RPG.Stats;
using Unity.Collections;
using Unity.Netcode;

namespace RPG.Quests {
    public class QuestStore : NetworkBehaviour, ISaveable, IPredicateHandler {
        private List<QuestState> _states = new();

        public IEnumerable<QuestState> States => _states;

        public event Action<QuestState> OnStateUpdated;

        public void AddQuest(Quest quest) {
            if (HasQuest(quest)) return;
            AddQuestServerRpc(quest.Title);
        }

        [ServerRpc]
        private void AddQuestServerRpc(FixedString512Bytes questName, ServerRpcParams serverRpcParams = default) {
            Quest quest = Quest.GetQuestByName(questName.Value);
            var state = new QuestState(this, quest);
            _states.Add(state);
            OnStateUpdated?.Invoke(state);
            ClientRpcParams clientRpcParams = new ClientRpcParams {
                Send = {
                    TargetClientIds = new[] { serverRpcParams.Receive.SenderClientId }
                }
            };
            AddQuestClientRpc(questName, clientRpcParams);
        }

        [ClientRpc]
        private void AddQuestClientRpc(FixedString512Bytes questName, ClientRpcParams clientRpcParams) {
            Quest quest = Quest.GetQuestByName(questName.Value);
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
            FinishQuestServerRpc(state.Quest.Title);
        }
        
        [ServerRpc]
        private void FinishQuestServerRpc(string questTitle, ServerRpcParams serverRpcParams = default) {
            var quest = Quest.GetQuestByName(questTitle);
            var state = GetQuestState(quest);
            _states.Remove(state);
            GiveAward(state.Quest);
            ClientRpcParams clientRpcParams = new ClientRpcParams {
                Send = {
                    TargetClientIds = new[] { serverRpcParams.Receive.SenderClientId }
                }
            };
            FinishQuestClientRpc(questTitle, clientRpcParams);
        }
        
        [ClientRpc]
        private void FinishQuestClientRpc(string questTitle, ClientRpcParams clientRpcParams) {
            var quest = Quest.GetQuestByName(questTitle);
            var state = GetQuestState(quest);
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
        public JToken CaptureAsJToken() {
            var quests = new JArray(
                    from state in _states
                    select new JObject(
                            new JProperty("quest_name", state.Quest),
                            new JProperty("completed_objectives", state.ObjectiveIndex)
                        )
                );
            return quests;
        }
        
        public void RestoreFromJToken(JToken state) {
            foreach (var quest in state) {
                _states.Add(
                    new QuestState(
                        this, 
                        Quest.GetQuestByName((string)quest["quest_name"]), 
                        (int)quest["completed_objectives"]
                    )
                );
            }
        }

        public object Predicate(string command, object[] arguments) {
            return command switch {
                "AddQuest" => AddQuestToPlayer((string)arguments[0]),
                _ => null
            };
        }

        private object AddQuestToPlayer(string questName) {
            AddQuest(Quest.GetQuestByName(questName));
            return 0;
        }
    }
}
