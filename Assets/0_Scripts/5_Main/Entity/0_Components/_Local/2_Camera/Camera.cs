using UnityEngine;

namespace Badbarbos.Player.Components
{
    public class Camera : EntityComponent
    {
        [SerializeField] private Transform _followTarget;
        [SerializeField] private Transform _cameraTransform;

        [SerializeField] private Vector3 _offset = new Vector3(0, 0.5f, 0.02f);

        [SerializeField] private float _sensX = 2f;
        [SerializeField] private float _sensY = 2f;
        [SerializeField] private float _sensitivityStep;
        [SerializeField] private float _smoothTime = 0.1f;
        [SerializeField] private float _tiltSensitivity = 0.1f;
        [SerializeField] private float _maxTiltAngle = 10f;

        private float _xRotation;
        private float _yRotation;
        private float _zTilt;

        private Vector2 _currentRotation;
        private Vector2 _rotationSmoothVelocity;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
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

            float deltaX = Input.GetAxis("Mouse X") * _sensX;
            float deltaY = Input.GetAxis("Mouse Y") * _sensY;

            _yRotation += deltaX;
            _xRotation -= deltaY;
            _xRotation = Mathf.Clamp(_xRotation, -75f, 75f);

            _zTilt = Mathf.Lerp(_zTilt, deltaX * _tiltSensitivity, Time.deltaTime * 5f);
        }

        private void LateUpdate()
        {
            _currentRotation = SmoothDampRotation(_currentRotation, new Vector2(_xRotation, _yRotation), ref _rotationSmoothVelocity, _smoothTime);

            Quaternion rotationY = Quaternion.Euler(0f, _currentRotation.y, 0f);
            Quaternion rotationX = Quaternion.Euler(_currentRotation.x, _currentRotation.y, 0f);
            float zTiltAngle = Mathf.Clamp(_zTilt, -_maxTiltAngle, _maxTiltAngle);
            Quaternion zTiltRotation = Quaternion.Euler(0f, 0f, zTiltAngle);

            _cameraTransform.rotation = rotationX * zTiltRotation;
            _followTarget.rotation = rotationY;

            _cameraTransform.position = _followTarget.position + _offset;
        }

        private Vector2 SmoothDampRotation(Vector2 current, Vector2 target, ref Vector2 velocity, float smoothTime)
        {
            float x = Mathf.SmoothDamp(current.x, target.x, ref velocity.x, smoothTime);
            float y = Mathf.SmoothDamp(current.y, target.y, ref velocity.y, smoothTime);
            return new Vector2(x, y);
        }
    }
}
