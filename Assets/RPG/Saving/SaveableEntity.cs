using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Unity.Collections;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using static RPG.Utils.Constants.DataConstants;

namespace RPG.Saving {
    [ExecuteAlways]
    public class SaveableEntity : NetworkBehaviour { 
        
        [SerializeField] NetworkVariable<FixedString64Bytes> uniqueIdentifier = new();
        public string EntityId => uniqueIdentifier.Value.Value;
            
        
        // CACHED STATE
        static readonly Dictionary<string, SaveableEntity> GlobalLookup = new();

        public JToken CaptureAsJToken() {
            JObject state = new JObject(new JProperty("content", new JObject()));
            JToken stateDict = state["content"];
            foreach (ISaveable jsonSaveable in GetComponents<ISaveable>()) {
                JToken token = jsonSaveable.CaptureAsJToken();
                string component = jsonSaveable.GetType().ToString();
                stateDict[component] = token;
            }
            return state;
        }

        public void RestoreFromJToken(JToken s) {
            if (IsServer) {
                uniqueIdentifier.Value = (string)s[PLAYER_ID];
            }
            if (s["content"] == null) return;
            JObject state = s["content"].ToObject<JObject>();
            IDictionary<string, JToken> stateDict = state;
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

            GlobalLookup[property.stringValue] = this;
        }
    #endif

        private bool IsUnique(string candidate) {
            if (!GlobalLookup.ContainsKey(candidate)) return true;
            if (GlobalLookup[candidate] == this) return true;
            if (GlobalLookup[candidate] == null) { 
                GlobalLookup.Remove(candidate);
                return true;
            }

            if (GlobalLookup[candidate].EntityId != candidate) {
                GlobalLookup.Remove(candidate);
                return true;
            }
            return false;
        }
    }
}
