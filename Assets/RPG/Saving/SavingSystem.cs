using System;
using System.Collections;
using static RPG.Utils.Constants.DataConstants;
using Newtonsoft.Json.Linq;
using RPG.Network.Controllers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPG.Saving {
    public class SavingSystem : MonoBehaviour {
        
        public void Save(SaveableEntity entity, string jwt, string sessionID) {
            var state = CaptureAsToken(entity, entity.UniqueID);
            PushStateToDataBase(state, jwt, sessionID);
        }
        private void PushStateToDataBase(JToken state, string token, string sessionID) {
            StartCoroutine(AuthenticationController.SaveEntity(state, token, sessionID));
        }

        public IEnumerator Load(GameObject loader, string playerId, string sessionId, Action<JObject> onLoadFinish) {
            yield return ContentController.GetPlayerContent(playerId, sessionId, data => {
                RestoreFromToken(data, loader);
                onLoadFinish(data);
            });
        }

        // PRIVATE

        private JToken CaptureAsToken(SaveableEntity entity, string idToSave) {
            var objectToSave = entity.CaptureAsJToken();
            objectToSave[PLAYER_ID] = idToSave;
            objectToSave[SCENE_INDEX] = SceneManager.GetActiveScene().buildIndex;
            return objectToSave;
        }


        private void RestoreFromToken(JObject state, GameObject loader) {
            SaveableEntity entity = loader.GetComponent<SaveableEntity>();
            if (!entity) return;
            entity.RestoreFromJToken(state);
        }
        
    }
}
