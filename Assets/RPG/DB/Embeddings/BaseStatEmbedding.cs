using Realms;
using RPG.Combat;
using RPG.Stats;

namespace RPG.DB.Embeddings {
    public class BaseStatEmbedding: EmbeddedObject {

        public CreatureClass CreatureClass {
            get => (CreatureClass)CreatureType;
            set => CreatureType = (int)value;
        }
        
        [MapTo("Level")]
        public int Level { get; set; }
        
        [MapTo("Experience")]
        public float Experience { get; set; }
        
        [MapTo("creature_class")]
        private int CreatureType { get; set; }

        public BaseStatEmbedding(BaseStats stats) {
            this.CreatureClass = stats.CreatureClass;
            this.Level = stats.Level;
            this.Experience = stats.Experience;
        }
        
        public BaseStatEmbedding() {}
    }
}
