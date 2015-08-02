using UnityEngine;
using System.Collections;

// TODO: Split Movement Engine from Movement Control Engine

public class NavMeshRigidBodyMovement : MonoBehaviour {

    [SerializeField] float _stationaryTurnSpeed = 180;
    [SerializeField] float _movingTurnSpeed = 360;
    [SerializeField] float _groundCheckDistance = 0.2f;
    [SerializeField] float _jumpPower = 5f;
    [SerializeField] float _gravityMultiplier = 2f;
    [SerializeField] float _animationAverageVelocity = 5.661f;

    NavMeshAgent _navMeshAgent;
    Rigidbody _rigidBody;
    Animator _animator;
    private float _turnAmount;
    private float _forwardAmount;
    Vector3 _groundNormal;
    float _previousGroundCheckDistance;
    private bool _isGrounded;
    private bool _isCrouching;
    private bool _isWalking;
    private bool _isJumping;

    void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();

        _navMeshAgent = GetComponent<NavMeshAgent>();
        _navMeshAgent.updatePosition = false;
        _navMeshAgent.updateRotation = true;

        _previousGroundCheckDistance = _groundCheckDistance;

        MainCameraControls.MovementEvent += OnMovement;
        MainCameraControls.CrouchEvent += OnCrouch;
        MainCameraControls.WalkEvent += OnWalk;
        MainCameraControls.JumpEvent += OnJump;
    }

    void OnDestroy()
    {
        MainCameraControls.MovementEvent -= OnMovement;
        MainCameraControls.CrouchEvent -= OnCrouch;
        MainCameraControls.WalkEvent -= OnWalk;
        MainCameraControls.JumpEvent -= OnJump;
    }

    void Update()
    {
        _navMeshAgent.nextPosition = transform.position;
        MoveUpdate(_navMeshAgent.velocity);
        AnimatorUpdate();
    }

    void OnAnimatorMove()
    {
        if (_isGrounded && Time.deltaTime > 0)
        {
            Vector3 animatorVelocity = _animator.deltaPosition / Time.deltaTime;
            Vector3 deltaVelocity = animatorVelocity - _rigidBody.velocity;
            deltaVelocity.y = 0;

            float mass = _rigidBody.mass;
            Vector3 force = mass * deltaVelocity / Time.deltaTime;

            float maxForce = _navMeshAgent.speed * 5;
            if (force.magnitude > maxForce)
                force = force.normalized * maxForce;

            _rigidBody.AddForce(force);
        }
    }

    void AnimatorUpdate()
    {
        
        _animator.SetFloat("Forward", _forwardAmount, 0.1f, Time.deltaTime);
        _animator.SetFloat("Turn", _turnAmount, 0.1f, Time.deltaTime);
        _animator.SetBool("Crouch", _isCrouching);
        _animator.SetBool("OnGround", _isGrounded);

        if (!_isGrounded)
            _animator.SetFloat("Jump", _rigidBody.velocity.y);

        if (_isGrounded && _rigidBody.velocity.sqrMagnitude > 0)
            _animator.speed = _navMeshAgent.speed / _animationAverageVelocity;
        else
            _animator.speed = 1;
    }

    

    void MoveUpdate(Vector3 move)
    {
        if (move.magnitude > 1f) move.Normalize();
        if (_isWalking) move *= 0.5f;
        move = transform.InverseTransformDirection(move);
        move = Vector3.ProjectOnPlane(move, _groundNormal);
        _turnAmount = Mathf.Atan2(move.x, move.z);
        _forwardAmount = move.z;

        CheckGroundStatus();
        ApplyExtraTurnRotation();

        if (_isGrounded)
            HandleGroundedMovement();
        else
            HandleAirborneMovement();
    }

    void HandleAirborneMovement()
    {
        Vector3 extraGravityForce = (Physics.gravity * _gravityMultiplier) - Physics.gravity;
        _rigidBody.AddForce(extraGravityForce);
        _groundCheckDistance = _rigidBody.velocity.y < 0 ? _previousGroundCheckDistance : 0.01f;
    }

    void HandleGroundedMovement()
    {
        if (_isJumping && !_isCrouching && _isGrounded)
        {
            _rigidBody.AddForce(new Vector3(0, _jumpPower, 0), ForceMode.Impulse);
            _isGrounded = false;
            _animator.applyRootMotion = false;
            _groundCheckDistance = 0.1f;
        }
    }

    void CheckGroundStatus()
    {
#if UNITY_EDITOR
        // helper to visualise the ground check ray in the scene view
        Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * _groundCheckDistance));
#endif
        // 0.1f is a small offset to start the ray from inside the character
        // it is also good to note that the transform position in the sample assets is at the base of the character
        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, _groundCheckDistance))
        {
            _groundNormal = hitInfo.normal;
            _isGrounded = true;
            _animator.applyRootMotion = true;
        }
        else
        {
            _isGrounded = false;
            _groundNormal = Vector3.up;
            _animator.applyRootMotion = false;
        }
    }

    void ApplyExtraTurnRotation()
    {
        // help the character turn faster (this is in addition to root rotation in the animation)
        float turnSpeed = Mathf.Lerp(_stationaryTurnSpeed, _movingTurnSpeed, _forwardAmount);
        transform.Rotate(0, _turnAmount * turnSpeed * Time.deltaTime, 0);
    }

    void OnMovement(Camera camera, Vector3 mousePosition)
    {
        RaycastHit hit;
        Ray ray = camera.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(ray, out hit))
            _navMeshAgent.SetDestination(hit.point);
    }

    void OnJump(bool isJumping)
    {
        _isJumping = isJumping;
    }

    void OnWalk(bool isWalking)
    {
        _isWalking = isWalking;
    }

    void OnCrouch(bool isCrouching)
    {
        _isCrouching = isCrouching;
    }
}
