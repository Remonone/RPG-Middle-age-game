using UnityEditor;
using UnityEngine;

namespace RPG.Dialogs.Editor {
    public class DialogEditor : EditorWindow {
        [MenuItem("GumuPeachu/Dialog Builder")]
        
        // TODO: Create preview, editing, addition, connection, deletion of dialog nodes.
        private static void ShowWindow() {
            var window = GetWindow<DialogEditor>();
            window.titleContent = new GUIContent("Dialog Builder");
            window.Show();
        }

        private void OnGUI() {
            
        }
    }
}
