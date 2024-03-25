using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
using static RPG.Utils.Constants.DataConstants;

namespace RPG.Saving {
    [ExecuteAlways]
    public class SaveableEntity : MonoBehaviour { 
        
        [SerializeField] string uniqueIdentifier = "";
        
        public string UniqueIdentifier {
            get { return uniqueIdentifier; }
            set { uniqueIdentifier = string.IsNullOrEmpty(uniqueIdentifier) ? value : uniqueIdentifier; }
        }
        
        // CACHED STATE
        static Dictionary<string, SaveableEntity> globalLookup = new();
        
        public string GetUniqueIdentifier() {
            return uniqueIdentifier;
        }
    
        public JToken CaptureAsJToken() {
            JObject state = new JObject();
            IDictionary<string, JToken> stateDict = state;
            foreach (ISaveable jsonSaveable in GetComponents<ISaveable>()) {
                JToken token = jsonSaveable.CaptureAsJToken();
                string component = jsonSaveable.GetType().ToString();
                stateDict[component] = token;
            }
            return state;
        }

        public void RestoreFromJToken(JToken s) {
            JObject state = s.ToObject<JObject>();
            IDictionary<string, JToken> stateDict = state;
            uniqueIdentifier = (string)stateDict[PLAYER_ID];
            foreach (ISaveable jsonSaveable in GetComponents<ISaveable>()) {
                string component = jsonSaveable.GetType().ToString();
                if (stateDict.ContainsKey(component)) {
                    jsonSaveable.RestoreFromJToken(stateDict[component]);
                }
            }
        }

    #if UNITY_EDITOR
        private void Update() {
            if (Application.IsPlaying(gameObject)) return;
            if (string.IsNullOrEmpty(gameObject.scene.path)) return;

            SerializedObject serializedObject = new SerializedObject(this);
            SerializedProperty property = serializedObject.FindProperty("uniqueIdentifier");
            
            if (string.IsNullOrEmpty(property.stringValue) || !IsUnique(property.stringValue)) {
                property.stringValue = System.Guid.NewGuid().ToString();
                serializedObject.ApplyModifiedProperties();
            }

            globalLookup[property.stringValue] = this;
        }
    #endif

        private bool IsUnique(string candidate) {
            if (!globalLookup.ContainsKey(candidate)) return true;
            if (globalLookup[candidate] == this) return true;
            if (globalLookup[candidate] == null) { 
                globalLookup.Remove(candidate);
                return true;
            }

            if (globalLookup[candidate].GetUniqueIdentifier() != candidate) {
                globalLookup.Remove(candidate);
                return true;
            }
            return false;
        }

    }
}
