using Realms;

namespace RPG.Models {
    public class InventoryModel: RealmObject {
        [PrimaryKey]
        public string InventoryID { get; set; }
    }
}
