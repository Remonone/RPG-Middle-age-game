using Unity.Netcode;

namespace RPG.Combat.DamageDefinition {
    public class DamageReport : INetworkSerializable {
        public float Damage;
        public DamageType Type;
        public NetworkObjectReference Attacker;
        public NetworkObjectReference Target;
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
            serializer.SerializeValue(ref Damage);
            serializer.SerializeValue(ref Type);
            serializer.SerializeValue(ref Attacker);
            serializer.SerializeValue(ref Target);
        }
    }
}
