using System;
using System.Collections.Generic;
using RPG.Stats.Relations;
using UnityEngine;

namespace RPG.Creatures.AI {
    [RequireComponent(typeof(Collider))]
    public class AiVision : MonoBehaviour {

        private IOrganisationWrapper _agent;
        private Organisation _organisation;

        private readonly List<GameObject> _enemiesInVision = new();
        private readonly List<GameObject> _creaturesInVision = new();

        public Action<GameObject> EnemySpotted;
        public Action<GameObject> CreatureSpotted;
        public Action<GameObject> EnemyMissing;
        public Action<GameObject> CreatureMissing;
        
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
            if (state.Value) OnEnemySpotted(obj);
            else OnCreatureSpotted(obj);
        }
        
        private void OnEnemySpotted(GameObject go) {
            _enemiesInVision.Add(go);
            EnemySpotted?.Invoke(go);
        }

        private void OnCreatureSpotted(GameObject go) {
            _creaturesInVision.Add(go);
            CreatureSpotted?.Invoke(go);
        }

        private void OnTriggerExit(Collider other) {
            var obj = other.gameObject;
            var state = IsHostileCreature(obj);
            if (state == null) return;
            if (state.Value)
                OnEnemyLeave(obj);
            else OnCreatureLeave(obj);
        }
        private void OnEnemyLeave(GameObject go) {
            _enemiesInVision.Remove(go);
            EnemyMissing?.Invoke(go);
        }
        private void OnCreatureLeave(GameObject go) {
            _creaturesInVision.Remove(go);
            CreatureMissing?.Invoke(go);
        }

        private bool? IsHostileCreature(GameObject obj) {
            if (!obj.TryGetComponent<IOrganisationWrapper>(out var agent)) return null;
            var organisation = agent.GetOrganisation();
            return _organisation.GetRelationWithOrganisation(organisation) < _organisation.AgroThreshold;
        }
    }
}
