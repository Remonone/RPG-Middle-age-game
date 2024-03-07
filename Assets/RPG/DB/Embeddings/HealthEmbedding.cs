using Realms;
using RPG.Combat;

namespace RPG.DB.Embeddings {
    public class HealthEmbedding : EmbeddedObject {
        public float CurrentHealth { get; private set; }
        public float MaxHealth { get; private set; }

        public HealthEmbedding(Health health) {
            this.CurrentHealth = health.CurrentHealth;
            this.MaxHealth = health.MaxHealth;
        }
        
        public HealthEmbedding() {}
    }
}
