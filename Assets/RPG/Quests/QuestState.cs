using System;
using System.Collections.Generic;
using RPG.Quests.Objectives;
using Object = UnityEngine.Object;

namespace RPG.Quests {
    [Serializable]
    public class QuestState {
        private Quest _quest;
        private List<QuestObjective> _objectives;
        private QuestStore _store;
        
        public Quest Quest => _quest;
        public bool IsCompleted => _objectives.Count < 1;
        
        public QuestState(QuestStore store, Quest quest) {
            _store = store;
            _quest = quest;
            InitQuest();
        }
        
        private void InitQuest() {
            foreach(var objective in _quest.Objectives) {
                var initObjective = Object.Instantiate(objective, _store.transform);
                _objectives.Add(initObjective);
            }
        }

    }
}
