using Cinemachine;
using UnityEngine;

namespace RPG.Creatures.Player {
    public class CameraBehaviour : MonoBehaviour {
        [SerializeField] private GameObject _followCamera;
        [SerializeField] private float _cameraRotationSpeed = .5f;

        private CinemachineVirtualCamera _camera;

        internal void Init() {
            _camera = Instantiate(_followCamera).GetComponent<CinemachineVirtualCamera>();
            _camera.Follow = transform;
            DontDestroyOnLoad(_camera);
        }

        private void OnDestroy() {
            Destroy(_camera);
        }

        public void RotateCamera(Vector2 delta) {
            var yMouseDelta = delta.x * _cameraRotationSpeed;
            _camera.transform.Rotate(Vector3.up, yMouseDelta, Space.World);
        }
    }
}
