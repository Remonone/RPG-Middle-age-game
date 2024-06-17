using System;
using UnityEngine;

namespace RPG.Quests.Objectives {
    public abstract class QuestObjective : MonoBehaviour {

        private bool _isQuestFinished;

        public event Action OnObjectiveCompeted;

        public abstract void Init(GameObject owner);

        protected void CompleteObjective() {
            if (!_isQuestFinished) {
                _isQuestFinished = true;
                OnObjectiveCompeted?.Invoke();
            }
        }
    }
}
