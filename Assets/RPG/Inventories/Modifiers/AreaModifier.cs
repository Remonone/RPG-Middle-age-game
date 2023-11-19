using System.Collections.Generic;
using System.Linq;
using RPG.Core.Predicate;
using RPG.Inventories.Items;
using UnityEngine;

namespace RPG.Inventories.Modifiers {
    [CreateAssetMenu(fileName = "FILENAME", menuName = "GumuPeachu/Create Modifier", order = 0)]
    public class AreaModifier : Modification {

        [SerializeField] private float _range;
        [SerializeField] private LayerMask _layer;
        [SerializeField] private string[] _prerequisites;
        [SerializeField] private string _performComponent;

        public override void PerformModification(GameObject holder, params string[] prerequisites) {
            if (!IsPrerequisitesSuite(prerequisites)) return;
            var holderPosition = holder.transform.position;
            var targets = GetTargetsInRange(holderPosition);
            foreach (var target in targets) {
                var component = target.GetComponent(_performComponent);
                if(component == null) continue;
                var predicateBehaviour = (PredicateMonoBehaviour)component;
                if(predicateBehaviour == null) continue;
                PredicateWorker.ParsePredicate(string.Format(_actionPredicate, predicateBehaviour.ComponentID), "all");
            }
        }

        private bool IsPrerequisitesSuite(string[] prerequisites) {
            return _prerequisites.All(prerequisites.Contains);
        }

        private Collider[] GetTargetsInRange(Vector3 position) {
            return Physics.OverlapSphere(position, _range, _layer);
        }
        
        
        
    }
}
