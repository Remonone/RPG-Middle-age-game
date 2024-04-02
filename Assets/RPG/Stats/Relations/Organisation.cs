
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Stats.Relations {
    [CreateAssetMenu(fileName = "Create Organisation", menuName = "GumuPeachu/Organisations/Create New Organisation", order = 0)]
    public class Organisation : ScriptableObject {
        [SerializeField] private string _name;
        [SerializeField] private string _description;
        [SerializeField] private string _localDescription;
        [SerializeField] private float _defaultRelation;
        [SerializeField] private Color _color;
        [SerializeField] private Image _flag;
        [SerializeField] private Sprite _emblem;
        [SerializeField] private float _agroThreshold;
        
        private readonly Dictionary<Organisation, float> _relations = new();

        public Sprite Emblem => _emblem;
        public Color Color => _color;

        public float AgroThreshold => _agroThreshold;

        public float GetRelationWithOrganisation(Organisation org) {
            if (!_relations.ContainsKey(org)) {
                _relations[org] = _defaultRelation;
            }
            return _relations[org];
        }
        
        
    }
}
