using Newtonsoft.Json.Linq;
using RPG.Lobby;
using RPG.Network.Model;
using RPG.Utils.Constants;

using static RPG.Utils.Constants.DataConstants;

namespace RPG.Network.Service {
    public class LobbyService {

        public static string ConvertPayloadToRequest(LobbyPayload payload) {
            if (payload.RoomID == null) return "";
            string path = $"{PropertyConstants.SERVER_DOMAIN}/{BackendCalls.FETCH_LOBBY}?id={payload.RoomID}";
            if (payload.Password != null) path += $"&password={payload.Password}";
            return path;
        }
        public static string CreateDataPayload(LobbyCreateData lobby) {
            JObject payload = new();
            payload["token"] = lobby.Token;
            payload["roomName"] = lobby.RoomName;
            payload["roomPassword"] = lobby.RoomPassword;
            payload["sessionId"] = lobby.SessionId;
            payload["ip"] = lobby.IP;
            payload["port"] = lobby.Port;
            return payload.ToString();
        }
        public static LobbyPack CreateLobbyPack(string receivedData) {
            var lobby = JToken.Parse(receivedData);
            var builder = LobbyPack.Builder.GetBuilder();
            builder.SetRoomID((string)lobby[LOBBY_ID]);
            builder.SetRoomName((string)lobby[LOBBY_NAME]);
            builder.SetPlayerCount((int)lobby[LOBBY_PLAYERS]);
            builder.SetMapName((string)lobby[LOBBY_MAP]);
            builder.SetIsSecured((bool)lobby[LOBBY_SECURED]);
            builder.SetLevel((int)lobby[LOBBY_LEVEL]);
            builder.SetHostName((string)lobby[LOBBY_HOST]);
            builder.SetSessionId((string)lobby[SESSION_ID]);
            return builder.Build();
        }
    }
}
