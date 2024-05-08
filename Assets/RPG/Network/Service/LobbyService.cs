using RPG.Network.Model;
using RPG.Utils.Constants;

namespace RPG.Network.Service {
    public class LobbyService {

        public static string ConvertPayloadToRequest(LobbyPayload payload) {
            if (payload.RoomID == null) return "";
            string path = $"{PropertyConstants.SERVER_DOMAIN}/{BackendCalls.FETCH_LOBBY}?id={payload.RoomID}";
            if (payload.Password != null) path += $"&password={payload.Password}";
            return path;
        }
    }
}
