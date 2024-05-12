namespace RPG.Lobby {
    public class LobbyPack {
        public string RoomName { get; private set; }
        public int PlayerCount { get; private set; }
        public bool IsSecured { get; private set; }
        public string MapName { get; private set; }
        public int Level { get; private set; }
        public ulong RoomID { get; private set; }
        
        public string HostName { get; private set; }

        public class Builder {
            private LobbyPack _lobbyPack = new();
        
            private Builder() {}

            public static Builder GetBuilder() {
                return new Builder();
            }

            public void SetRoomName(string roomName) => _lobbyPack.RoomName = roomName;
            public void SetPlayerCount(int count) => _lobbyPack.PlayerCount = count;
            public void SetIsSecured(bool isSecured) => _lobbyPack.IsSecured = isSecured;
            public void SetMapName(string mapName) => _lobbyPack.MapName = mapName;
            public void SetLevel(int level) => _lobbyPack.Level = level;
            public void SetRoomID(ulong id) => _lobbyPack.RoomID = id;

            public void SetHostName(string host) => _lobbyPack.HostName = host;
            
            public LobbyPack Build() {
                return _lobbyPack;
            }
        }
    }
}

