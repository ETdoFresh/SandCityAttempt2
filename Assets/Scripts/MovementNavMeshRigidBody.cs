using UnityEngine;
using System.Collections;

public class MovementNavMeshRigidBody : MonoBehaviour
{

    public float stationaryTurnSpeed = 180;
    public float movingTurnSpeed = 360;
    public float groundCheckDistance = 0.5f;
    public float jumpPower = 5f;
    public float gravityMultiplier = 2f;
    public float animationRunVelocity = 5.661f;
    public float animationWalkVelocity = 1.556f;
    public float animationCrouchVelocity = 0.560f;
    public RuntimeAnimatorController animatorController;

    UnityEngine.AI.NavMeshAgent _navMeshAgent;
    Rigidbody _rigidBody;
    public Animator _animator;
    float _turnAmount;
    float _forwardAmount;
    Vector3 _groundNormal;
    float _initialGroundCheckDistance;
    bool _isGrounded;
    bool _isCrouching;
    bool _isWalking;
    bool _isJumping;
    Vector3 _sphereCheckCenter;
    float _radius;
    CharacterFootStep _characterFootStep;

    void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        _characterFootStep = GetComponentInChildren<CharacterFootStep>();

        _navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        _navMeshAgent.updatePosition = false;
        _navMeshAgent.updateRotation = false;

        _initialGroundCheckDistance = groundCheckDistance;
    }

    void OnEnable()
    {
        if (_navMeshAgent != null) _navMeshAgent.SetDestination(transform.position);
        ControlThirdPersonCamera.OnMovement += MovementCallback;
        ControlThirdPersonCamera.OnCrouch += CrouchCallback;
        ControlThirdPersonCamera.OnWalk += WalkCallback;
        ControlThirdPersonCamera.OnJump += JumpCallback;
    }

    void OnDisable()
    {
        ControlThirdPersonCamera.OnMovement -= MovementCallback;
        ControlThirdPersonCamera.OnCrouch -= CrouchCallback;
        ControlThirdPersonCamera.OnWalk -= WalkCallback;
        ControlThirdPersonCamera.OnJump -= JumpCallback;
    }

    void Update()
    {
        _navMeshAgent.nextPosition = transform.position;
        MoveUpdate(_navMeshAgent.velocity);
        AnimatorUpdate();
    }

    void FixedUpdate()
    {
        Vector3 navMeshVelocity = _navMeshAgent.velocity;
        if (_isCrouching) navMeshVelocity *= 0.3f;
        else if (_isWalking) navMeshVelocity *= 0.5f;
        if (!_isGrounded) return;

        float mass = _rigidBody.mass;
        Vector3 deltaVelocity = navMeshVelocity - _rigidBody.velocity;
        deltaVelocity.y = 0;
        Vector3 force = mass * deltaVelocity / Time.fixedDeltaTime;
        _rigidBody.AddForce(force);
    }

    void AnimatorUpdate()
    {
        float absoluteForwardAmount = Mathf.Abs(_forwardAmount);
        if (absoluteForwardAmount == 0)
            _animator.speed = 1;
        else if (_isCrouching)
        {
            float transitionSpeed = animationCrouchVelocity * absoluteForwardAmount * 2;
            _animator.speed = _rigidBody.velocity.magnitude / transitionSpeed;
        }
        else if (absoluteForwardAmount <= 0.5f)
        {
            float transitionSpeed = animationWalkVelocity * absoluteForwardAmount * 2;
            _animator.speed = _rigidBody.velocity.magnitude / transitionSpeed;
        }
        else if (absoluteForwardAmount > 0.5f)
        {
            float slope = (animationRunVelocity - animationWalkVelocity) / (1 - 0.5f);
            float offset = animationRunVelocity - 1 * slope;
            float transitionSpeed = absoluteForwardAmount * slope + offset;
            _animator.speed = _rigidBody.velocity.magnitude / transitionSpeed;
        }

        _animator.SetFloat("Forward", _forwardAmount, 0.1f, Time.deltaTime);
        _animator.SetFloat("Turn", _turnAmount, 0.1f, Time.deltaTime);
        _animator.SetBool("Crouch", _isCrouching);
        _animator.SetBool("OnGround", _isGrounded);

        if (!_isGrounded)
            _animator.SetFloat("Jump", _rigidBody.velocity.y);
    }

    void MoveUpdate(Vector3 move)
    {
        if (move.magnitude > 1f) move.Normalize();
        if (_isWalking && !_isCrouching) move *= 0.5f;
        move = transform.InverseTransformDirection(move);
        _turnAmount = Mathf.Atan2(move.x, move.z);
        _forwardAmount = move.z;

        float minimumTimeBetweenSteps = 0.2f;
        _characterFootStep.timeBetweenSteps = float.PositiveInfinity;
        if (_forwardAmount != 0)
        {
            if (_isCrouching)
                _characterFootStep.timeBetweenSteps = minimumTimeBetweenSteps / 0.3f;
            else if (_isGrounded)
                _characterFootStep.timeBetweenSteps = minimumTimeBetweenSteps;
        }

        CheckGroundStatus();
        ApplyExtraTurnRotation();

        if (_isGrounded)
            HandleGroundedMovement();
        else
            HandleAirborneMovement();
    }

    void HandleAirborneMovement()
    {
        Vector3 extraGravityForce = (Physics.gravity * gravityMultiplier) - Physics.gravity;
        _rigidBody.AddForce(extraGravityForce);
        groundCheckDistance = _rigidBody.velocity.y < 0 ? _initialGroundCheckDistance : 0;
    }

    void HandleGroundedMovement()
    {
        if (_isJumping && !_isCrouching && _isGrounded)
        {
            _rigidBody.AddForce(new Vector3(0, jumpPower, 0), ForceMode.Impulse);
            _isGrounded = false;
            groundCheckDistance = 0.1f;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(_sphereCheckCenter, _radius);
    }

    void CheckGroundStatus()
    {
        // it is also good to note that the transform position in the sample assets is at the base of the character
        _sphereCheckCenter = transform.position + (Vector3.up * _radius) + (Vector3.down * groundCheckDistance);
        _radius = 0.5f;
        int layerMask = ~LayerMask.GetMask("Player");

        _isGrounded = Physics.CheckSphere(_sphereCheckCenter, _radius, layerMask);
    }

    void ApplyExtraTurnRotation()
    {
        // help the character turn faster (this is in addition to root rotation in the animation)
        float turnSpeed = Mathf.Lerp(stationaryTurnSpeed, movingTurnSpeed, _forwardAmount);
        transform.Rotate(0, _turnAmount * turnSpeed * Time.deltaTime, 0);
    }

    void MovementCallback(Camera camera, Vector3 mousePosition)
    {
        RaycastHit hit;
        Ray ray = camera.ScreenPointToRay(mousePosition);
        LayerMask layerMask = (1 << LayerMask.NameToLayer("RaycastInput"));
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            _navMeshAgent.SetDestination(hit.point);
    }

    void JumpCallback(bool isJumping)
    {
        _isJumping = isJumping;
    }

    void WalkCallback(bool isWalking)
    {
        _isWalking = isWalking;
    }

    void CrouchCallback(bool isCrouching)
    {
        _isCrouching = isCrouching;
    }
}
