using UnityEngine.UIElements;

namespace RPG.UI.Elements {
    public class WorldRow : VisualElement {
        public string Map { get; set; }
        public int Level { get; set; }
        public string Name { get; set; }
        
        public new class UxmlFactory : UxmlFactory<WorldRow, UxmlTraits> {}

        public new class UxmlTraits : VisualElement.UxmlTraits {
            private UxmlIntAttributeDescription _level = new() { name = "level" };
            private UxmlStringAttributeDescription _map = new() { name = "map" };
            private UxmlStringAttributeDescription _name = new() { name = "world_name" };

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc) {
                base.Init(ve, bag, cc);
                var row = ve as WorldRow;
                row.Clear();

                row.Name = _name.GetValueFromBag(bag, cc);
                row.Add(new Label(row.Name));

                row.Map = _map.GetValueFromBag(bag, cc);
                row.Add(new Label(row.Map));

                row.Level = _level.GetValueFromBag(bag, cc);
                row.Add(new Label($"{row.Level}"));
            }
        }
    }
}
