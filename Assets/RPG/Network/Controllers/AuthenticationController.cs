using System;
using System.Collections;
using Newtonsoft.Json.Linq;
using RPG.Network.Service;
using RPG.Utils.Constants;
using UnityEngine;
using UnityEngine.Networking;

namespace RPG.Network.Controllers {
    public static class AuthenticationController {
        public static IEnumerator SignIn(string login, string password, Action<JToken> onLogin, Action<JToken> onFail, int attemptCount = 10) {
           string result = "";
            for (int i = 0; i < attemptCount && string.IsNullOrEmpty(result); i++) {
                using (UnityWebRequest www = UnityWebRequest.Get($"{PropertyConstants.SERVER_DOMAIN}/{BackendCalls.FETCH_USER}?login={login}&password={password}")) {
                    yield return www.SendWebRequest();
                    switch (www.result) {
                        case UnityWebRequest.Result.Success:
                            result = www.downloadHandler.text;
                            break;
                        case UnityWebRequest.Result.DataProcessingError:
                            
                            result = www.downloadHandler.text;
                            onFail(JToken.Parse(result));
                            yield break;
                        case UnityWebRequest.Result.ConnectionError:
                            break;
                    }
                }
            }

            if (string.IsNullOrEmpty(result)) {
                onFail(null);
                yield break;
            }
            JToken data = JToken.Parse(result);
            
            onLogin(data);
        }

        public static IEnumerator SignUp(string login, string username, string password, string serverName, Action<JToken> onAuth, int attemptCount = 10) {
            string result = "";
            bool isFailed = false;
            string query = UserService.ConvertUserToForm(login, username, password, serverName);

            for (int i = 0; i < attemptCount && !isFailed; i++) {
                using (UnityWebRequest www = UnityWebRequest.Post($"{PropertyConstants.SERVER_DOMAIN}/{BackendCalls.REGISTER_USER}", query, "application/json")) {
                    yield return www.SendWebRequest();
                    switch (www.result) {
                        case UnityWebRequest.Result.Success:
                            result = www.downloadHandler.text;
                            break;
                        case UnityWebRequest.Result.DataProcessingError:
                            result = www.downloadHandler.text;
                            isFailed = true;
                            break;
                        case UnityWebRequest.Result.ConnectionError:
                            break;
                    }

                    if (!string.IsNullOrEmpty(result)) break;
                }
            }
            if (string.IsNullOrEmpty(result)) {
                Debug.LogError("Connection Timeout...");
                yield break;
            }
            JToken token = JObject.Parse(result);
            if (token["error_message"] != null) {
                Debug.LogError((string)token["error_message"]);
                yield break;
            }

            onAuth(token);
        }

        public static IEnumerator SaveEntity(JToken entityToSave, string token, int attemptCount = 10) {
            bool isFailed = false;
            for (int i = 0; i < attemptCount && !isFailed; i++) {
                using (UnityWebRequest www = UnityWebRequest.Put($"{PropertyConstants.SERVER_DOMAIN}/{BackendCalls.SAVE_USER}?token={token}", entityToSave.ToString())) {
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
        }

        public static IEnumerator LoadEntity(string jwt, Action<JObject> onData) {
            using (UnityWebRequest www = UnityWebRequest.Get($"{PropertyConstants.SERVER_DOMAIN}/{BackendCalls.LOAD_USER}?jwt={jwt}")) {
                yield return www.SendWebRequest();
                switch (www.result) {
                    case UnityWebRequest.Result.Success:
                        JObject obj = JObject.Parse(www.downloadHandler.text);
                        onData(obj);
                        break;
                    case UnityWebRequest.Result.DataProcessingError:
                        Debug.LogError(www.error);
                        onData(null);
                        break;
                    case UnityWebRequest.Result.ConnectionError:
                        Debug.LogError("Connection timeout...");
                        break;
                }
            }
        }
    }
}
