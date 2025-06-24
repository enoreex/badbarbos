using UnityEngine;

namespace Badbarbos.Player.Components
{
    public class CameraThird : EntityComponent
    {
        [SerializeField] private Transform _followTarget;
        [SerializeField] private Transform _cameraTransform;

        [SerializeField] private Vector3 _offset = new Vector3(0f, 2f, -4f);
        [SerializeField] private float _minPitch = -40f;
        [SerializeField] private float _maxPitch = 75f;

        [SerializeField] private float _sensX = 2f;
        [SerializeField] private float _sensY = 2f;
        [SerializeField] private float _sensitivityStep = 0.1f;
        [SerializeField] private float _smoothTime = 0.1f;

        [SerializeField] private LayerMask _collisionMask = ~0;
        [SerializeField] private float _collisionRadius = 0.3f;
        [SerializeField] private float _collisionOffset = 0.2f;

        private float _yaw;
        private float _pitch;

        private Vector2 _currentRot;
        private Vector2 _rotVelocity;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            _currentRot = new Vector2(_cameraTransform.localEulerAngles.x, _cameraTransform.eulerAngles.y);

            _pitch = _currentRot.x;
            _yaw = _currentRot.y;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (Cursor.lockState == CursorLockMode.Locked)
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
            }

            if (Input.GetKeyDown(KeyCode.Equals))
            {
                _sensX += _sensitivityStep;
                _sensY += _sensitivityStep;
            }
            else if (Input.GetKeyDown(KeyCode.Minus))
            {
                _sensX = Mathf.Max(0.1f, _sensX - _sensitivityStep);
                _sensY = Mathf.Max(0.1f, _sensY - _sensitivityStep);
            }
        }

        private void LateUpdate()
        {
            float deltaX = Input.GetAxis("Mouse X") * _sensX;
            float deltaY = Input.GetAxis("Mouse Y") * _sensY;

            _yaw += deltaX;

            _pitch -= deltaY;
            _pitch = Mathf.Clamp(_pitch, _minPitch, _maxPitch);

            Vector2 targetRot = new Vector2(_pitch, _yaw);

            _currentRot = Vector2.SmoothDamp(_currentRot, targetRot, ref _rotVelocity, _smoothTime);

            Quaternion camRot = Quaternion.Euler(_currentRot.x, _currentRot.y, 0f);
            Vector3 desiredPos = _followTarget.position + camRot * _offset;

            Vector3 dir = (desiredPos - _followTarget.position).normalized;
            float dist = Vector3.Distance(_followTarget.position, desiredPos);
            Vector3 finalPos = desiredPos;
            if (Physics.SphereCast(_followTarget.position, _collisionRadius, dir, out RaycastHit hit, dist, _collisionMask))
            {
                finalPos = _followTarget.position + dir * (hit.distance - _collisionOffset);
            }

            _cameraTransform.position = finalPos;
            _cameraTransform.rotation = camRot;

            _followTarget.rotation = Quaternion.Euler(0f, _currentRot.y, 0f);
        }
    }
}
