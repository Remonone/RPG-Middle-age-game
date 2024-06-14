using System;
using System.Collections.Generic;
using RPG.Combat;
using RPG.Stats.Relations;
using UnityEngine;

namespace RPG.Creatures.AI {
    [RequireComponent(typeof(Collider))]
    public class AiVision : MonoBehaviour {

        private OrganisationWrapper _agent;
        private Organisation _organisation;

        private readonly List<Health> _enemiesInVision = new();
        private readonly List<Health> _creaturesInVision = new();

        public Action<Health> EnemySpotted;
        public Action<Health> CreatureSpotted;
        public Action<Health> EnemyMissing;
        public Action<Health> CreatureMissing;
        
        public IEnumerator<Health> EnemiesInVision => _enemiesInVision.GetEnumerator();
        public IEnumerable<Health> CreaturesInVision => _creaturesInVision;

        public bool IsEnemiesInVision => _enemiesInVision.Count > 0;
        public bool IsCreaturesInVision => _creaturesInVision.Count > 0;

        private void Awake() {
            _agent = GetComponentInParent<OrganisationWrapper>();
            _organisation = _agent.Organisation;
        }

        public void OnTriggerEnter(Collider other) {
            var obj = other.gameObject;
            var state = IsHostileCreature(obj);
            if (state == null) return;
            var health = obj.GetComponent<Health>();
            if (!health.IsAlive) return;
            if (state.Value) OnEnemySpotted(health);
            else OnCreatureSpotted(health);
        }
        
        private void OnEnemySpotted(Health health) {
            health.OnDie += OnEnemyDead;
            _enemiesInVision.Add(health);
            EnemySpotted?.Invoke(health);
        }

        private void OnCreatureSpotted(Health health) {
            _creaturesInVision.Add(health);
            CreatureSpotted?.Invoke(health);
        }

        private void OnTriggerExit(Collider other) {
            var obj = other.gameObject;
            var state = IsHostileCreature(obj);
            if (state == null) return;
            var health = obj.GetComponent<Health>();
            if (state.Value)
                OnEnemyLeave(health);
            else OnCreatureLeave(health);
        }
        private void OnEnemyLeave(Health health) {
            _enemiesInVision.Remove(health);
            EnemyMissing?.Invoke(health);
            health.GetComponent<Health>().OnDie -= OnEnemyDead;
        }
        private void OnCreatureLeave(Health health) {
            _creaturesInVision.Remove(health);
            CreatureMissing?.Invoke(health);
        }

        private void OnEnemyDead() {
            foreach (var obj in _enemiesInVision) {
                var health = obj.GetComponent<Health>();
                if(health.IsAlive) continue;
                health.OnDie -= OnEnemyDead;
                EnemyMissing?.Invoke(obj);
                _enemiesInVision.Remove(obj);
                break;
            }
        }

        private bool? IsHostileCreature(GameObject obj) {
            if (!obj.TryGetComponent<OrganisationWrapper>(out var agent)) return null;
            var organisation = agent.Organisation;
            return _organisation.GetRelationWithOrganisation(organisation) < _organisation.AgroThreshold;
        }
    }
}
