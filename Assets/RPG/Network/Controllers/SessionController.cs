using System.Collections;
using Newtonsoft.Json.Linq;
using RPG.Network.Model;
using RPG.Utils.Constants;
using UnityEngine;
using UnityEngine.Networking;

namespace RPG.Network.Controllers {
    public static class SessionController {
        public static IEnumerator UpdateSession(UpdateSession session, string token, string sessionID) {
            bool isFailed = false;
            string formattedBody = FormatSessionBody(session);
            using (UnityWebRequest www = UnityWebRequest.Put($"{PropertyConstants.SERVER_DOMAIN}/{BackendCalls.UPDATE_WORLD}?token={token}&session={sessionID}", formattedBody)) {
                www.SetRequestHeader("Content-Type", "application/json");
                yield return www.SendWebRequest();
                switch (www.result) {
                    case UnityWebRequest.Result.Success:
                        yield break;
                    case UnityWebRequest.Result.ConnectionError:
                        break;
                    case UnityWebRequest.Result.DataProcessingError:
                        isFailed = true;
                        break;
                }
            }

            if (isFailed) {
                Debug.LogError("Error during saving an entity");
            }
        }

        private static string FormatSessionBody(UpdateSession session) {
            JToken sessionData = new JObject();
            sessionData["map"] = session.SessionMap;
            sessionData["level"] = session.SessionLevel;
            return sessionData.ToString();
        }
    }
}