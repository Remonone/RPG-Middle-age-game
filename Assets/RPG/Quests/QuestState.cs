using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPG.Quests {
    [Serializable]
    public class QuestState {
        [SerializeField] private Quest _quest;
        [SerializeField] private List<Objective> _completedObjectives = new();

        public Quest Quest => _quest;

        public bool IsCompleted => _quest.Objectives.All(objective => _completedObjectives.Contains(objective));
        
        public QuestState(Quest quest) {
            _quest = quest;
        }

        public QuestState(Quest quest, List<Objective> completedObjectives) {
            _quest = quest;
            _completedObjectives = completedObjectives;
        }
        
        public bool IsObjectiveCompleted(Objective obj) => _completedObjectives.Contains(obj);
        
        public void CompleteObjective(string objectiveId) {
            if (_quest.HasObjective(objectiveId)) return;
            _completedObjectives.Add(_quest.Objectives.Single(obj => obj.Id == objectiveId));
        }
    }
}
