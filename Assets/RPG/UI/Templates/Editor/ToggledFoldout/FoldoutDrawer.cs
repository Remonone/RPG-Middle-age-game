using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace RPG.UI.Templates.Editor.ToggledFoldout {
    [CustomPropertyDrawer(typeof(FoldoutAttribute))]
    public class FoldoutDrawer : PropertyDrawer {
        // TODO: Add property to disable a property
        private VisualElement _foldout;
        public override VisualElement CreatePropertyGUI( SerializedProperty property ) {
            VisualTreeAsset uiAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/RPG/UI/Templates/Editor/ToggledFoldout/Foldout.uxml");
            VisualElement container = uiAsset.Instantiate();
            _foldout = container;
            InstantiateContainer(container, property);
            return container;
        }
        private void InstantiateContainer(VisualElement container, SerializedProperty property) {
            Toggle active = container.Q<Toggle>("Active");
            Toggle showcase = container.Q<Toggle>("Showcase");
            container.Q<Label>().text = property.displayName;
            var content = container.Q<VisualElement>("content");
            if (property.hasChildren) {
                var iterator = property.Copy(); 
                iterator.Next(true);
                do {
                    content.Add(GetPropertyElement(iterator));
                } while (iterator.NextVisible(false));
            } else {
                content.Add(GetPropertyElement(property));
            }
            
            active.RegisterValueChangedCallback(OnChangeActive);
            showcase.RegisterValueChangedCallback(OnShowcaseChange);
        }
        private VisualElement GetPropertyElement(SerializedProperty serializedProperty) {
            var name = serializedProperty.displayName;
            
            // TODO: Create a visualisation for a list, array or dictionary;
            return serializedProperty.propertyType switch {
                SerializedPropertyType.Boolean => new Toggle(name),
                SerializedPropertyType.Float => new FloatField(name),
                SerializedPropertyType.Integer => new IntegerField(name),
                SerializedPropertyType.Hash128 => new Hash128Field(name),
                SerializedPropertyType.Rect => new RectField(name),
                SerializedPropertyType.Bounds => new BoundsField(name),
                SerializedPropertyType.Enum => new EnumField(name),
                SerializedPropertyType.Gradient => new GradientField(name),
                SerializedPropertyType.Vector2 => new Vector2Field(name),
                SerializedPropertyType.Vector3 => new Vector3Field(name),
                SerializedPropertyType.Vector4 => new Vector4Field(name),
                SerializedPropertyType.Vector2Int => new Vector2IntField(name),
                SerializedPropertyType.Vector3Int => new Vector3IntField(name),
                SerializedPropertyType.Color => new ColorField(name),
                SerializedPropertyType.AnimationCurve => new CurveField(name),
                SerializedPropertyType.LayerMask => new MaskField(name),
                SerializedPropertyType.String => new TextField(name),
                SerializedPropertyType.ObjectReference => new ObjectField(name), 
                SerializedPropertyType.Character => new TextField(name),
                SerializedPropertyType.RectInt => new RectIntField(name),
                SerializedPropertyType.BoundsInt => new BoundsIntField(name),
                _ => new Label("Property type not supported!")
            };
        }
        private void OnShowcaseChange(ChangeEvent<bool> evt) {
            _foldout.Q<VisualElement>("content").ToggleInClassList("gp-foldout__content_visible");
        }
        private void OnChangeActive(ChangeEvent<bool> evt) {
            
        }
    }
}
