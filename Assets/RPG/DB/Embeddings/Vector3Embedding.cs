using Realms;
using UnityEngine;

namespace RPG.DB.Embeddings {
    public class Vector3Embedding : EmbeddedObject 
    {
        public float X { get; private set; }
        public float Y { get; private set; }
        public float Z { get; private set; }

        public Vector3Embedding(Vector3 vector) {
            X = vector.x;
            Y = vector.y;
            Z = vector.z;
        }

        public Vector3 ToVector3() => new Vector3(X, Y, Z);
        
        public Vector3Embedding() {}

    }
}
