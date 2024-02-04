using System.Collections.Generic;
using RPG.Creatures.AI.Roles;
using RPG.Stats.Relations;
using UnityEngine;

namespace RPG.Creatures.AI {
    [RequireComponent(typeof(Collider))]
    public class AiVision : MonoBehaviour {

        private IOrganisationWrapper _agent;
        private Organisation _organisation;

        private readonly List<GameObject> _enemiesInVision = new();
        private readonly List<GameObject> _creaturesInVision = new();

        public IEnumerator<GameObject> EnemiesInVision => _enemiesInVision.GetEnumerator();
        public IEnumerable<GameObject> CreaturesInVision => _creaturesInVision;

        public bool IsEnemiesInVision => _enemiesInVision.Count > 0;
        public bool IsCreaturesInVision => _creaturesInVision.Count > 0;

        private void Awake() {
            _agent = GetComponentInParent<IOrganisationWrapper>();
            _organisation = _agent.GetOrganisation();
        }

        public void OnTriggerEnter(Collider other) {
            var obj = other.gameObject;
            var state = IsHostileCreature(obj);
            if (state == null) return;
            print("Enemy in vision");
            if(state.Value) _enemiesInVision.Add(obj);
                else _creaturesInVision.Add(other.gameObject);
        }

        private void OnTriggerExit(Collider other) {
            _enemiesInVision.Remove(other.gameObject);
            _creaturesInVision.Remove(other.gameObject);
        }

        private bool? IsHostileCreature(GameObject obj) {
            if (!obj.TryGetComponent<IOrganisationWrapper>(out var agent)) return null;
            var organisation = agent.GetOrganisation();
            return _organisation.GetRelationWithOrganisation(organisation) < _organisation.AgroThreshold;
        }
    }
}
