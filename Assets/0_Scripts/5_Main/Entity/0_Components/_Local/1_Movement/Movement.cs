using UnityEngine;
using UnityEngine.Events;

namespace Badbarbos.Player.Components.Movement
{
    public class Movement : EntityComponent
    {
        [Header("Common")]
        [SerializeField] private Transform _direction;
        [SerializeField] private Transform _rotationLink;
        [SerializeField] private Rigidbody _rigidbody;

        private float _horizontalInput, _verticalInput, _moveSpeed, _lastYAngle, _rotationSpeed;
        private Vector3 _moveDirection;

        [Header("Movement")]
        [SerializeField] private float _walkSpeed, _sprintSpeed, _groundDrag;

        [Header("Jumping")]
        [SerializeField] private float _jumpForce, _jumpCooldown, _airMultiplier;
        private bool _readyToJump = true, _wantJump, _justJumped;

        [Header("Ground Check")]
        [SerializeField] private float _playerHeight;
        [SerializeField] private LayerMask _whatIsGround;

        private bool _isGround;

        [Header("Slope Handling")]
        [SerializeField] private float _maxSlopeAngle;

        private RaycastHit _slopeHit;
        private bool _exitingSlope;

        [Header("Bunny Hop")]
        [SerializeField] private float _bhopMultiplier = 1.05f, _maxBhopSpeed = 20f, _speedDecayRate = 1f;

        private float _baseWalkSpeed, _baseSprintSpeed;

        [Header("Braking")]
        [SerializeField] private float _brakeRate = 5f;

        [Header("Handlers")]
        [SerializeField] private UnityEvent<bool> _onGroundNotify;
        [SerializeField] private UnityEvent _onSprintNotify, _onWalkNotify;
        [SerializeField] private UnityEvent<Vector2> _moveNotify;

        private bool _groundNotify, _sprintNotify, _walkNotify, _sprintPressed;

        private void Start()
        {
            _rigidbody.freezeRotation = true;
            _baseWalkSpeed = _walkSpeed;
            _baseSprintSpeed = _sprintSpeed;
            if (_rotationLink != null) _lastYAngle = _rotationLink.eulerAngles.y;
        }

        private void Update()
        {
            if (Input.GetButtonDown("Jump") && _readyToJump && _isGround) _wantJump = true;
            _horizontalInput = Input.GetAxisRaw("Horizontal");
            _verticalInput = Input.GetAxisRaw("Vertical");
            _moveNotify.Invoke(new Vector2(_horizontalInput, _verticalInput));
            _sprintPressed = Input.GetKey(KeyCode.LeftShift);
        }

        private void FixedUpdate()
        {
            if (_rotationLink != null)
            {
                float currentY = _rotationLink.eulerAngles.y;
                _rotationSpeed = Mathf.DeltaAngle(_lastYAngle, currentY) / Time.fixedDeltaTime;
                _lastYAngle = currentY;
            }

            _isGround = Physics.Raycast(transform.position, Vector3.down, _playerHeight * 0.5f + 0.2f, _whatIsGround);
            _rigidbody.drag = _isGround ? _groundDrag : 0f;

            if (_wantJump)
            {
                _wantJump = false; _readyToJump = false; _exitingSlope = true; _justJumped = true;
                _rigidbody.velocity = new Vector3(0, 0, 0);
                _rigidbody.AddForce(transform.up * _jumpForce, ForceMode.Impulse);
                bool validDir = (_horizontalInput > 0f && _rotationSpeed > 50f)
                             || (_horizontalInput < 0f && _rotationSpeed < -50f);
                if (validDir)
                {
                    _walkSpeed = Mathf.Min(_walkSpeed * _bhopMultiplier, _maxBhopSpeed);
                    _sprintSpeed = Mathf.Min(_sprintSpeed * _bhopMultiplier, _maxBhopSpeed);
                }
                Invoke(nameof(ResetJump), _jumpCooldown);
            }

            _moveDirection = _direction.forward * _verticalInput + _direction.right * _horizontalInput;
            bool onSlope = Physics.Raycast(transform.position, Vector3.down,
                    out _slopeHit, _playerHeight * 0.5f + 0.3f)
                && Vector3.Angle(Vector3.up, _slopeHit.normal) < _maxSlopeAngle
                && !_exitingSlope;

            if (_isGround && _horizontalInput == 0f && _verticalInput == 0f)
            {
                Vector3 flat = new Vector3(_rigidbody.velocity.x, 0f, _rigidbody.velocity.z);
                Vector3 braked = Vector3.Lerp(flat, Vector3.zero, _brakeRate * Time.fixedDeltaTime);
                _rigidbody.velocity = new Vector3(braked.x, _rigidbody.velocity.y, braked.z);
            }
            else if (onSlope)
            {
                Vector3 proj = Vector3.ProjectOnPlane(_moveDirection, _slopeHit.normal).normalized;
                _rigidbody.AddForce(20f * _moveSpeed * proj, ForceMode.Force);
            }
            else if (_isGround)
                _rigidbody.AddForce(10f * _moveSpeed * _moveDirection.normalized, ForceMode.Force);
            else
                _rigidbody.AddForce(10f * _airMultiplier * _moveSpeed * _moveDirection.normalized, ForceMode.Force);

            _rigidbody.useGravity = !(onSlope);
            ControlSpeed();

            if (_isGround && !_exitingSlope && !_justJumped)
            {
                _walkSpeed = Mathf.Lerp(_walkSpeed, _baseWalkSpeed, _speedDecayRate * Time.fixedDeltaTime);
                _sprintSpeed = Mathf.Lerp(_sprintSpeed, _baseSprintSpeed, _speedDecayRate * Time.fixedDeltaTime);
            }

            if (_isGround != _groundNotify) { _onGroundNotify.Invoke(_isGround); _groundNotify = _isGround; }
            if (_isGround && _sprintPressed)
            {
                if (!_sprintNotify) { _sprintNotify = true; _onSprintNotify.Invoke(); }
                _walkNotify = false;
            }
            else if (_isGround)
            {
                if (!_walkNotify) { _walkNotify = true; _onWalkNotify.Invoke(); }
                _sprintNotify = false;
            }

            _justJumped = false;
        }

        private void ResetJump() { _readyToJump = true; _exitingSlope = false; }

        private void ControlSpeed()
        {
            _moveSpeed = _isGround ? (_sprintPressed ? _sprintSpeed : _walkSpeed) : _moveSpeed;
            if (_rigidbody.velocity.magnitude > _moveSpeed)
            {
                Vector3 flat = new Vector3(
                    _rigidbody.velocity.x, 0f, _rigidbody.velocity.z)
                    .normalized * _moveSpeed;
                _rigidbody.velocity = new Vector3(flat.x, _rigidbody.velocity.y, flat.z);
            }
        }
    }
}
