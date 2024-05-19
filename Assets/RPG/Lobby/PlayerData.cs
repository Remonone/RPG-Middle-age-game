using System;
using Unity.Collections;
using Unity.Netcode;

namespace RPG.Lobby {
    public struct PlayerData : IEquatable<PlayerData>, INetworkSerializable {
        public FixedString64Bytes PlayerID;
        public FixedString64Bytes Username;

        public PlayerData(string id, string username) {
            this.PlayerID = id;
            this.Username = username;
        }
        public bool Equals(PlayerData other) {
            return other.Username == Username && other.PlayerID == PlayerID;

        }
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
            serializer.SerializeValue(ref PlayerID);
            serializer.SerializeValue(ref Username);
        }
    }
}
