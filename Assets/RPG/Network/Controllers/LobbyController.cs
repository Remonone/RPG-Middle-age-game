using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using RPG.Lobby;
using RPG.Network.Model;
using RPG.Network.Service;
using RPG.Utils.Constants;
using UnityEngine.Networking;

using static RPG.Utils.Constants.DataConstants;

namespace RPG.Network.Controllers {
    public static class LobbyController {
        public static IEnumerator GetLobbyList(Action<List<LobbyPack>> onLobbies, Action<string> onFail) {
            string result = @"{}";
            using (UnityWebRequest www = UnityWebRequest.Get($"{PropertyConstants.SERVER_DOMAIN}/{BackendCalls.LOAD_LOBBIES}")) {
                yield return www.SendWebRequest();
                switch (www.result) {
                    case UnityWebRequest.Result.Success:
                        result = www.downloadHandler.text;
                        break;
                    case UnityWebRequest.Result.DataProcessingError:
                        result = www.downloadHandler.text;
                        break;
                }

                JToken lobbieData = JToken.Parse(result);
                if (!lobbieData.HasValues) {
                    onLobbies.Invoke(new List<LobbyPack>());
                    yield break;
                }

                if (lobbieData.Value<string>(ERROR_MESSAGE) != null) {
                    onFail.Invoke((string)lobbieData[ERROR_MESSAGE]);
                }

                List<LobbyPack> packs = new List<LobbyPack>();
                foreach (var lobby in lobbieData) {
                    LobbyPack pack = LobbyService.CreateLobbyPack(lobby.ToString());
                    packs.Add(pack);
                }
                
                onLobbies.Invoke(packs);
            }
        }
        public static IEnumerator JoinLobby(LobbyPayload payload, Action<string> onConnect, Action<string> onFail) {
            using (UnityWebRequest www = UnityWebRequest.Get(LobbyService.ConvertPayloadToRequest(payload))) {
                yield return www.SendWebRequest();
                if (www.result is UnityWebRequest.Result.ConnectionError) {
                    onFail.Invoke(www.error);
                    yield break;
                }

                if (www.result is UnityWebRequest.Result.ProtocolError) {
                    onFail.Invoke(www.downloadHandler.text);
                    yield break;
                }
                
                onConnect.Invoke(www.downloadHandler.text);
            }
        }

        public static IEnumerator CreateLobby(LobbyCreateData lobby, Action<LobbyPack> onCreate, Action<JToken> onFail) {
            string query = LobbyService.CreateDataPayload(lobby);
            using (UnityWebRequest www = UnityWebRequest.Post($"{PropertyConstants.SERVER_DOMAIN}/{BackendCalls.CREATE_LOBBY}", query, "application/json")) {
                yield return www.SendWebRequest();
                if (www.result is UnityWebRequest.Result.ConnectionError) {
                    onFail.Invoke(www.error);
                    yield break;
                }

                if (www.result is UnityWebRequest.Result.DataProcessingError) {
                    onFail.Invoke(JToken.Parse(www.downloadHandler.text));
                    yield break;
                }

                if (www.result is UnityWebRequest.Result.Success) {
                    var lobbyData = LobbyService.CreateLobbyPack(www.downloadHandler.text);
                    onCreate.Invoke(lobbyData);
                }
            }
        }
    }
}
