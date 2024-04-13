using RPG.UI.Elements.Input;
using UnityEngine;

namespace RPG.Utils.UI {
    public static class DocumentUtils {
        public static bool CheckOnEmptyValues(params ValueInput[] fields) {
            foreach (var field in fields) {
                Debug.Log(field.Value);
                if (string.IsNullOrEmpty(field.Value)) return true;
            }
            return false;
        }
    }
}
