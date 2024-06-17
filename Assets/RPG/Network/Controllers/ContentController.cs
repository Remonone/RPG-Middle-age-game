using System;
using System.Collections;
using Newtonsoft.Json.Linq;
using RPG.Utils.Constants;
using UnityEngine;
using UnityEngine.Networking;

namespace RPG.Network.Controllers {
    public static class ContentController {

        public static IEnumerator GetPlayerContent(string playerId, string sessionId, Action<JObject> onData) {
            using (UnityWebRequest www = UnityWebRequest.Get($"{PropertyConstants.SERVER_DOMAIN}/{BackendCalls.GET_CONTENT}?playerId={playerId}&sessionId={sessionId}")) {
                yield return www.SendWebRequest();
                if (www.result is UnityWebRequest.Result.Success) {
                    var result = www.downloadHandler.text;
                    onData.Invoke(JObject.Parse(result));
                    yield break;
                }
                onData.Invoke(JObject.Parse(@"{}"));
                
            }
        }
    }
}
