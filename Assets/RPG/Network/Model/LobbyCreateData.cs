namespace RPG.Network.Model {
    public record LobbyCreateData(string SessionId, string Token, string RoomPassword, string RoomName, string IP, ushort Port);
}
