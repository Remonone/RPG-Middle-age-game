using System;
using System.Linq;
using RPG.Quests;
using UnityEngine;
using UnityEngine.UIElements;

namespace RPG.UI.Quest {
    public class QuestUI : MonoBehaviour {
        [SerializeField] private QuestStore _store;
        [SerializeField] private UIDocument _questBlock;

        private VisualElement _questContainer;
        
        private void Start() {
            _store.OnStateUpdated += StoreOnStateUpdated;
        }

        private void StoreOnStateUpdated() {
            var quests = _store.States;
            var states = quests.Select(quest => CreateQuestState(quest));
            
        }

        private VisualElement CreateQuestState(QuestState quest) {
            return new();
        }
    }
}
