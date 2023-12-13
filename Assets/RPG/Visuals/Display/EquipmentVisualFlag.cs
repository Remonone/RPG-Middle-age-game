using RPG.Inventories;
using UnityEngine;

namespace RPG.Visuals.Display {
    public class EquipmentVisualFlag : MonoBehaviour {
        [SerializeField] private EquipmentSlot _slot;

        public EquipmentSlot Slot => _slot;
    }
}
