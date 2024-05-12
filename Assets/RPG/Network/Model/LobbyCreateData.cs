namespace RPG.Network.Model {
    public record LobbyCreateData(string sessionId, string token, string roomPassword, string roomName);
}
