using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace Badbarbos.Player.Components
{
    public class Animation : EntityComponent
    {
        [SerializeField] private Transform _targetDirectorGetter;
        [SerializeField] private Transform _targetRotatorGetter;
        [SerializeField] private Transform _targetRotatorSetter;
        [SerializeField] private Animator _animator;

        [Header("Input Keys")]
        [SerializeField] private string _inputXKey;
        [SerializeField] private string _inputYKey;
        [SerializeField] private string _isGroundKey;

        [Header("Smoothing & Speed Settings")]
        [SerializeField] private float _smoothness = 10f;
        [SerializeField] private float _minAnimSpeed = 0.8f;
        [SerializeField] private float _maxAnimSpeed = 1.2f;
        [SerializeField] private float _maxRunSpeed = 6f;

        [SerializeField] private float _animSpeedWalk = 1f;
        [SerializeField] private float _animSpeedRun = 1f;

        private float _animSpeed;
        private Vector3 _playerPreviousPosition;
        private float _maxSpeed;
        private bool _isGround;

        void Start()
        {
            _playerPreviousPosition = _targetDirectorGetter.position;
            _maxSpeed = 1f;
        }

        void FixedUpdate()
        {
            Vector3 currentPos = _targetDirectorGetter.position;
            Vector3 movement = currentPos - _playerPreviousPosition;
            Vector3 flatDir = new Vector3(movement.x, 0, movement.z).normalized;

            Vector3 lookDir = _targetRotatorGetter.forward;

            float playerSpeed = movement.magnitude / Time.deltaTime;

            float inputX = Vector3.Dot(flatDir, lookDir);
            float inputY = -Vector3.Dot(flatDir, Vector3.Cross(lookDir, Vector3.up));

            inputX = Mathf.Clamp(inputX, -_maxSpeed, _maxSpeed);
            inputY = Mathf.Clamp(inputY, -_maxSpeed, _maxSpeed);

            if (playerSpeed < 1f)
            {
                inputX = 0f;
                inputY = 0f;
            }

            float smoothX = SmoothInterpolate(_animator.GetFloat(_inputXKey), inputY, _smoothness);
            float smoothY = SmoothInterpolate(_animator.GetFloat(_inputYKey), inputX, _smoothness);

            _animator.SetFloat(_inputXKey, smoothX);
            _animator.SetFloat(_inputYKey, smoothY);

            float t = Mathf.Clamp01(playerSpeed / _maxRunSpeed * _animSpeed);
            _animator.speed = Mathf.Lerp(_minAnimSpeed, _maxAnimSpeed, t);

            _playerPreviousPosition = currentPos;
            _targetRotatorSetter.forward = lookDir;

            _animator.SetBool(_isGroundKey, _isGround);

            Debug.DrawRay(currentPos, flatDir * 5f, Color.green);
        }

        private float SmoothInterpolate(float from, float to, float smoothness)
        {
            return Mathf.Lerp(from, to, 1f - Mathf.Exp(-smoothness * Time.deltaTime));
        }

        public void SetGround(bool value) => _isGround = value;

        public void OnSprint()
        {
            _maxSpeed = 1f;
            _animSpeed = _animSpeedRun;
        }

        public void OnWalk()
        {
            _maxSpeed = 0.5f;
            _animSpeed = _animSpeedWalk;
        }
    }
}
