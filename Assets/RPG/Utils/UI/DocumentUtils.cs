using UnityEngine.UIElements;

namespace RPG.Utils.UI {
    public static class DocumentUtils {
        public static bool CheckOnEmptyValues(params TextField[] fields) {
            foreach (var field in fields) {
                if (string.IsNullOrEmpty(field.value)) return true;
            }
            return false;
        }
    }
}
