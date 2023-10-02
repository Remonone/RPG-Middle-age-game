using UnityEditor;
using UnityEngine;

namespace RPG.Utils.Editor {
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property,
            GUIContent label) {
            using (var scope = new EditorGUI.DisabledGroupScope(true)) {
                EditorGUI.LabelField(position, label.text + ":", FormatValue(property));
            }
        }

        private string FormatValue(SerializedProperty prop) {
            return prop.propertyType switch {
                SerializedPropertyType.Boolean => prop.boolValue.ToString(),
                SerializedPropertyType.Integer => prop.intValue.ToString(),
                SerializedPropertyType.String => prop.stringValue,
                SerializedPropertyType.Float => prop.floatValue.ToString("0.00"),
                _ => ""
            };
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return base.GetPropertyHeight(property, label);
        }
    }
}
