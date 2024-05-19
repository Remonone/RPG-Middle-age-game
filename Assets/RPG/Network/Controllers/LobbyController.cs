using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using RPG.Lobby;
using RPG.Network.Model;
using RPG.Network.Service;
using RPG.UI.Create;
using RPG.Utils.Constants;
using UnityEngine;
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

                if (lobbieData.Value<JArray>("lobbies") == null ) {
                    onFail.Invoke((string)lobbieData[ERROR_MESSAGE]);
                }

                List<LobbyPack> packs = new List<LobbyPack>();
                foreach (var lobby in lobbieData["lobbies"]) {
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

        public static IEnumerator CreateGame(string token, string worldName, Action<SessionData> onCreate, Action<string> onFail) {
            using (UnityWebRequest www = UnityWebRequest.Get($"{PropertyConstants.SERVER_DOMAIN}/{BackendCalls.CREATE_WORLD}?token={token}&name={worldName}")) {
                yield return www.SendWebRequest();
                if (www.result is UnityWebRequest.Result.ConnectionError) {
                    onFail.Invoke("Some issue during sending the request...");
                    yield break;
                }

                if (www.result is UnityWebRequest.Result.DataProcessingError) {
                    var data = JToken.Parse(www.downloadHandler.text);
                    onFail.Invoke((string)data["error_message"]);
                    yield break;
                }

                if (www.result is UnityWebRequest.Result.Success) {
                    var data = JToken.Parse(www.downloadHandler.text);
                    Debug.Log(www.downloadHandler.text);
                    var session = new SessionData((string)data["_id"], (string)data["host_id"],
                        (string)data["session_map"], (int)data["level"], (string)data["session_name"]);
                    onCreate.Invoke(session);
                }
            }
        }

        public static IEnumerator FetchWorlds(string token, Action<List<SessionData>> onFetch, Action<string> onFail) {
            using (UnityWebRequest www = UnityWebRequest.Get($"{PropertyConstants.SERVER_DOMAIN}/{BackendCalls.FETCH_WORLDS}?token={token}")) {
                yield return www.SendWebRequest();
                if (www.result is UnityWebRequest.Result.ConnectionError) {
                    onFail.Invoke("Some issue during sending the request...");
                    yield break;
                }

                if (www.result is UnityWebRequest.Result.DataProcessingError) {
                    var data = JToken.Parse(www.downloadHandler.text);
                    onFail.Invoke((string)data["error_message"]);
                    yield break;
                }
                
                if (www.result is UnityWebRequest.Result.Success) {
                    var data = JToken.Parse(www.downloadHandler.text);
                    List<SessionData> sessions = new();
                    var worlds = data["sessions"];
                    foreach (var session in worlds) {
                        SessionData ses = new SessionData((string)session["_id"], (string)session["host_id"],
                            (string)session["session_map"], (int)session["level"], (string)session["session_name"]);
                        sessions.Add(ses);
                    }
                    onFetch.Invoke(sessions);
                }
            }
        }

        public static IEnumerator UnloadLobby(string token, string roomId) {
            using (UnityWebRequest www = UnityWebRequest.Delete($"{PropertyConstants.SERVER_DOMAIN}/{BackendCalls.DELETE_LOBBY}?token={token}&roomId={roomId}")) {
                yield return www.SendWebRequest();
            }
        }
    }
}
