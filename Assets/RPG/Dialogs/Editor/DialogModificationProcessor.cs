using System.IO;
using UnityEditor;

namespace RPG.Dialogs.Editor {
    public class DialogModificationProcessor: AssetModificationProcessor {
        
        private static AssetMoveResult OnWillMoveAsset(string sourcePath, string destinationPath) {
            var dialogue = AssetDatabase.LoadMainAssetAtPath(sourcePath) as Dialog;
            if (dialogue == null) {
                return AssetMoveResult.DidNotMove;
            }

            if (Path.GetDirectoryName(sourcePath) != Path.GetDirectoryName(destinationPath)) {
                return AssetMoveResult.DidNotMove;
            }
            dialogue.name = Path.GetFileNameWithoutExtension(destinationPath);
            return AssetMoveResult.DidNotMove;
        }
    }
}
