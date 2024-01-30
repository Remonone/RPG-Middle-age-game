using System.Collections.Generic;
using RPG.Stats.Relations;
using UnityEngine;

namespace RPG.Creatures.AI {
    [RequireComponent(typeof(Collider))]
    public class AiVision : MonoBehaviour {


        private List<GameObject> _objectsInVision = new();
        
        public void OnTriggerEnter(Collider other) {
            _objectsInVision.Add(other.gameObject);
        }

        private void OnTriggerExit(Collider other) {
            _objectsInVision.Remove(other.gameObject);
        }

        public Dictionary<Organisation, GameObject[]> GetTargetsInVision() {
            var dict = new Dictionary<Organisation, List<GameObject>>();
            foreach (var obj in _objectsInVision) {
                if (!obj.TryGetComponent<IOrganisationWrapper>(out var agent)) continue;
                var organisation = agent.GetOrganisation();
                if(!dict.ContainsKey(organisation)) dict.Add(organisation, new List<GameObject>());
                dict[organisation].Add(gameObject);
            }
            var result = new Dictionary<Organisation, GameObject[]>();
            foreach (var pair in dict) {
                result[pair.Key] = pair.Value.ToArray();
            }
            return result;
        }
    }
}
