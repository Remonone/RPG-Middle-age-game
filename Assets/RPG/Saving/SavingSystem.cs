using System;
using System.Collections;
using static RPG.Utils.Constants.DataConstants;
using Newtonsoft.Json.Linq;
using RPG.Network.Controllers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPG.Saving {
    public class SavingSystem : MonoBehaviour {
        
        public void Save(SaveableEntity entity, string uniqueId, string jwt) {
            var state = CaptureAsToken(entity, uniqueId);
            PushStateToDataBase(state, jwt);
        }
        private void PushStateToDataBase(JToken state, string token) {
            StartCoroutine(AuthenticationController.SaveEntity(state, token));
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
