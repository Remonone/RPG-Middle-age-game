using System.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RPG.Network.Client;
using RPG.Network.Service;
using RPG.Utils;
using UnityEngine;
using UnityEngine.Networking;

namespace RPG.Network.Controllers {
    public static class AuthenticationController {
        public static IEnumerator SignIn(string login, string password, int attemptCount = 10) {
            bool isFailed = false;
            string result = "";
            
            for (int i = 0; i < attemptCount && !isFailed && !string.IsNullOrEmpty(result); i++) {
                using (UnityWebRequest www = UnityWebRequest.Get(
                           $"{PropertyConstants.SERVER_DOMAIN}:{PropertyConstants.BACKEND_PORT}/{BackendCalls.FETCH_USER}?login={login}&password={password}")) {
                    yield return www.SendWebRequest();
                    switch (www.result) {
                        case UnityWebRequest.Result.Success:
                            result = www.downloadHandler.text;
                            break;
                        case UnityWebRequest.Result.DataProcessingError:
                            isFailed = true;
                            break;
                        case UnityWebRequest.Result.ConnectionError:
                            break;
                    }
                }
            }

            if (string.IsNullOrEmpty(result)) {
                Debug.LogError(isFailed ? "Error during authentication..." : 
                    "Connection Timeout.");
                yield break;
            }
            ClientSingleton.Instance.Manager.SetData(JToken.Parse(result));
            
        }

        public static IEnumerator SignUp(string login, string username, string password, int attemptCount = 10) {
            string result = "";
            bool isFailed = false;
            string query = UserService.ConvertUserToForm(login, username, password);
            
            for (int i = 0; i < attemptCount && !isFailed; i++) {
                using (UnityWebRequest www = UnityWebRequest.PostWwwForm($"{PropertyConstants.SERVER_DOMAIN}/{BackendCalls.REGISTER_USER}", query)) {
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
            
            ClientSingleton.Instance.Manager.SetData(token);
        }

        public static IEnumerator SaveEntity(string uniqueID, JToken entityToSave, int attemtCount = 10) {
            bool isFailed = false;
            using (UnityWebRequest www = UnityWebRequest.Put($"{PropertyConstants.BACKEND_HOST}:{PropertyConstants.BACKEND_PORT}/{BackendCalls.SAVE_USER}?uniqueId={uniqueID}", JsonConvert.ToString(entityToSave))) {
                for (int i = 0; i < attemtCount && !isFailed; i++) {
                    yield return www.SendWebRequest();
                    switch (www.result) {
                        case UnityWebRequest.Result.Success:
                        case UnityWebRequest.Result.ConnectionError:
                            break;
                        case UnityWebRequest.Result.ProtocolError:
                            isFailed = true;
                            break;
                    }
                }

                if (isFailed) {
                    Debug.LogError("Error during saving an entity");
                }
            }
        }
    }
}
