using RPG.Combat;
using RPG.Stats;
using RPG.Stats.Relations;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using ColorUtility = UnityEngine.ColorUtility;

namespace RPG.UI.InfoBar {
    public class HpBar : NetworkBehaviour {

        private const string RESOURCE_PATH = "Organisation/Emblems/";

        [SerializeField] private Image _color;
        [SerializeField] private Image _emblem;
        [SerializeField] private TextMeshProUGUI _username;
        [SerializeField] private TextMeshProUGUI _level;
        [SerializeField] private RectTransform _hpSize;
        [SerializeField] private RectTransform _currentHp;
        [SerializeField] private Vector3 _offset;
        

        private readonly NetworkVariable<FixedString32Bytes> _colorHex = new();
        private readonly NetworkVariable<FixedString64Bytes> _emblemImagePath = new();
        private readonly NetworkVariable<float> _healthPercent = new();
        private readonly NetworkVariable<FixedString64Bytes> _nameValue = new();
        private readonly NetworkVariable<FixedString32Bytes> _levelValue = new();
        private readonly NetworkVariable<bool> _isInitialized = new();
        
        private Health _healthBar;
        private BaseStats _stats;
        private GameObject _holder;

        public override void OnNetworkSpawn() {
            base.OnNetworkSpawn();
            if (IsClient && !IsOwner) {
                Init(); 
            }
        }


        public void StartInit(string username, GameObject holder) {
            if (IsOwner) {
                InitFromServerRpc(username, holder.GetComponent<NetworkBehaviourReference>());
                gameObject.SetActive(false);
            }
            
            _healthBar = holder.GetComponent<Health>();
            _stats = holder.GetComponent<BaseStats>();
            _holder = holder;
        }

        [ServerRpc]
        private void InitFromServerRpc(FixedString64Bytes username, NetworkBehaviourReference reference) {
            reference.TryGet(out var netObj);
            var organisation = netObj.GetComponent<IOrganisationWrapper>().GetOrganisation();
            var emblem = RESOURCE_PATH + organisation.Emblem.texture.name;
            var color = organisation.Color;
            
            _nameValue.Value = username;
            _colorHex.Value = ColorUtility.ToHtmlStringRGBA(color);
            _emblemImagePath.Value = emblem;

            _healthBar = netObj.GetComponent<Health>();
            _healthBar.OnHealthChanged += UpdateHealthValue;
            
            _stats = netObj.GetComponent<BaseStats>();
            _stats.OnLevelUp += UpdateLevelValue;
            UpdateLevelValue();
            _isInitialized.Value = true;
        }

        private void UpdateLevelValue() {
            _levelValue.Value = _stats.Level < 10 ? "0" + _stats.Level : _stats.Level.ToString();
        }
        
        private void UpdateHealthValue(float health) {
            _healthPercent.Value = _healthBar.CurrentHealth / _healthBar.MaxHealth;
        }
        
        private void Init() {
            _colorHex.OnValueChanged += OnSetColor;
            _emblemImagePath.OnValueChanged += OnEmblemChanged;
            _nameValue.OnValueChanged += OnNameSet;
            _healthPercent.OnValueChanged += UpdateHealth;
            _levelValue.OnValueChanged += UpdateLevel;

            if (!_isInitialized.Value) return;
            _username.text = _nameValue.Value.Value;
            OnSetColor(new FixedString32Bytes(), _colorHex.Value);
            OnEmblemChanged(new FixedString64Bytes(), _emblemImagePath.Value);
            UpdateHealth(0, 1);
            UpdateLevel(new FixedString32Bytes(), _levelValue.Value);
        }
        private void OnNameSet(FixedString64Bytes previousValue, FixedString64Bytes newValue) {
            _username.text = newValue.Value;
        }
        private void OnEmblemChanged(FixedString64Bytes previousValue, FixedString64Bytes newValue) {
            _emblem.sprite = Resources.Load<Sprite>(newValue.Value);
        }
        private void OnSetColor(FixedString32Bytes previousValue, FixedString32Bytes newValue) {
            ColorUtility.TryParseHtmlString(newValue.Value, out var color);
            _color.color = color;
        }

        private void UpdateLevel(FixedString32Bytes oldValue, FixedString32Bytes newValue) {
            _level.text = newValue.Value;
        }

        private void UpdateHealth(float oldValue, float newValue) {
            _currentHp.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,_hpSize.rect.width * newValue);
        }

        private void Update() {
            transform.position = _holder.transform.position + _offset;
        }

        // [ClientRpc]
        // private void UpdatePositionClientRpc() {
        //     transform.rotation.SetLookRotation(Camera.current.transform.position - transform.position);
        // }
    }
}