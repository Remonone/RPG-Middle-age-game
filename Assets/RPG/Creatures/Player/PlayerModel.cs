using Realms;
using RPG.DB.Embeddings;
using UnityEngine;

namespace RPG.Creatures.Player {
    public class PlayerModel : RealmObject {
        [PrimaryKey] 
        [MapTo("ID")]
        public string PlayerID { get; set; }
        [MapTo("login")]
        public string PlayerLogin { get; set; }
        [MapTo("password")]
        public string HashedPassword { get; set; }

        [MapTo("role")] 
        public string PlayerRole { get; set; }

        public Vector3 Position {
            get => PlayerPosition.ToVector3();
            set => PlayerPosition = new Vector3Embedding(value);
        }

        [MapTo("stats")]
        public BaseStatEmbedding Stats { get; set; }
        
        [MapTo("health")]
        public HealthEmbedding Health { get; set; }
        
        [MapTo("position")]
        private Vector3Embedding PlayerPosition { get; set; }
    }
}
