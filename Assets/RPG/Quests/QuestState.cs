using System;
using System.Collections.Generic;
using RPG.Quests.Objectives;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RPG.Quests {
    [Serializable]
    public class QuestState {
        private Quest _quest;
        private Stack<QuestObjective> _objectivesList = new();
        private QuestStore _store;

        private QuestObjective _currentObjective;
        private int _objectiveIndex;

        public bool IsCompleted => _objectivesList.Count < 1;
        public int ObjectiveIndex => _objectiveIndex;
        
        public Quest Quest => _quest;
        
        public QuestState(QuestStore store, Quest quest) {
            _store = store;
            _quest = quest;
            InitQuest();
        }
        
        public QuestState(QuestStore store, Quest quest, int objective) {
            _store = store;
            _quest = quest;
            InitQuest(objective);
        }

        private void InitQuest(int objectiveIndex = 0) {
            foreach(var objective in _quest.Objectives) _objectivesList.Push(objective);
            for (int i = 0; i < objectiveIndex - 1; i++) _objectivesList.Pop();
            var task = _objectivesList.Pop();
            var instantiatedTask = Object.Instantiate(task);
            instantiatedTask.Init(_store.gameObject);
            _currentObjective = instantiatedTask;
            _currentObjective.OnObjectiveCompeted += OnObjectiveFinished;
        }
        
        
        private void OnObjectiveFinished() {
            _objectiveIndex++;
            _store.UpdateState(this);
            _currentObjective.OnObjectiveCompeted -= OnObjectiveFinished;
            Object.Destroy(_currentObjective.gameObject);
            if (!_objectivesList.TryPop(out var task)) return;
            _currentObjective = task;
        }

    }
}
