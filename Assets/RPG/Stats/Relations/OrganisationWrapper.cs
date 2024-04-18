using UnityEngine;

namespace RPG.Stats.Relations {
    public class OrganisationWrapper : MonoBehaviour {
        [SerializeField] private Organisation _organisation;

        public Organisation Organisation => _organisation;
    }
}
