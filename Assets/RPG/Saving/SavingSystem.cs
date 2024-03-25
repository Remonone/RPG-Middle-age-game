using System.Linq;
using static RPG.Utils.Constants.DataConstants;
using Newtonsoft.Json.Linq;
using RPG.Creatures.Player;
using RPG.Network.Controllers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPG.Saving {
    public class SavingSystem : MonoBehaviour {
        
        public void Save(string uniqueId) {
            var state = CaptureAsToken(uniqueId);
            PushStateToDataBase(state);
        }
        private void PushStateToDataBase(JToken state) {
            StartCoroutine(AuthenticationController.SaveEntity(state));
        }

        public void Load(PlayerController loader, string jwt) {
            StartCoroutine(AuthenticationController.LoadEntity(jwt, data => {
                RestoreFromToken(data, loader.gameObject);
                loader.Init(data);
            }));
        }

        // PRIVATE


        private JToken CaptureAsToken(string idToSave) {
            SaveableEntity entity = FindObjectsOfType<SaveableEntity>().First(obj => obj.UniqueIdentifier == idToSave);
            var objectToSave = entity.CaptureAsJToken();
            objectToSave[PLAYER_ID] = idToSave;
            objectToSave[SCENE_INDEX] = SceneManager.GetActiveScene().buildIndex;
            return objectToSave;
        }


        private bool RestoreFromToken(JObject state, GameObject loader) {
            SaveableEntity entity = loader.GetComponent<SaveableEntity>();
            if (!entity) return false;
            entity.RestoreFromJToken(state);
            return true;
        }
        
    }
}
