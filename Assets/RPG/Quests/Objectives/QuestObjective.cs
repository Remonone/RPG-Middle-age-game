using UnityEngine;

namespace RPG.Quests.Objectives {
    public abstract class QuestObjective : MonoBehaviour {

        private bool _isQuestFinished;

        protected void CompleteObjective() {
            if (!_isQuestFinished) {
                _isQuestFinished = true;
                Destroy(gameObject);
            }
        }
    }
}
