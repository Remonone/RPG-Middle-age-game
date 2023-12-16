using UnityEditor;
using UnityEngine;

namespace RPG.Core.Predicate.Editor {
    public class PredicateBuilderEditor : EditorWindow {
        [MenuItem("GumuPeachu/Predicate Builder")]
        // TODO: Make predicate editor as dialog editor
        private static void ShowWindow() {
            var window = GetWindow<PredicateBuilderEditor>();
            window.titleContent = new GUIContent("Predicate Builder");
            window.Show();
        }

        private void OnGUI() {
            
        }
    }
}
