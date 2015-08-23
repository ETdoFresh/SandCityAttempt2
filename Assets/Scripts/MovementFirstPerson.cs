using UnityEngine;
using System.Collections;

public class MovementFirstPerson : MonoBehaviour
{
    public float groundCheckDistance = 0.5f;
    public float jumpPower = 5f;
    public float gravityMultiplier = 2f;
    public float animationAverageVelocity = 5.661f;
    public RuntimeAnimatorController animatorController;
    public Animator _animator;

    Rigidbody _rigidBody;
    NavMeshAgent _navMeshAgent;
    public Vector2 _movement;
    Vector3 _groundNormal;
    float _initialGroundCheckDistance;
    bool _isGrounded;
    bool _isCrouching;
    bool _isWalking;
    bool _isJumping;
    Vector3 _sphereCheckCenter;
    float _radius;

    void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        _navMeshAgent = GetComponent<NavMeshAgent>();

        _initialGroundCheckDistance = groundCheckDistance;
    }

    void OnEnable()
    {
        if (_animator != null) _animator.speed = 1;
        ControlFirstPersonMovement.OnMovement += MovementCallback;
        ControlFirstPersonMovement.OnCrouch += CrouchCallback;
        ControlFirstPersonMovement.OnWalk += WalkCallback;
        ControlFirstPersonMovement.OnJump += JumpCallback;
    }

    void OnDisable()
    {
        ControlFirstPersonMovement.OnMovement -= MovementCallback;
        ControlFirstPersonMovement.OnCrouch -= CrouchCallback;
        ControlFirstPersonMovement.OnWalk -= WalkCallback;
        ControlFirstPersonMovement.OnJump -= JumpCallback;
    }

    void Update()
    {
        MoveUpdate();
        AnimatorUpdate();
    }

    void FixedUpdate()
    {
        if (!_isGrounded) return;
        Vector3 velocity = _movement * _navMeshAgent.speed;
        velocity.z = velocity.y;
        velocity = transform.TransformVector(velocity);
        float mass = _rigidBody.mass;
        Vector3 deltaVelocity = velocity - _rigidBody.velocity;
        Vector3 force = mass * deltaVelocity / Time.fixedDeltaTime;
        force.y = 0;
        _rigidBody.AddForce(force);
    }

    void AnimatorUpdate()
    {
        _animator.SetFloat("Vertical", _movement.y, 0.1f, Time.deltaTime);
        _animator.SetFloat("Horizontal", _movement.x, 0.1f, Time.deltaTime);
        _animator.SetBool("Crouch", _isCrouching);
        _animator.SetBool("OnGround", _isGrounded);

        if (!_isGrounded)
            _animator.SetFloat("Jump", _rigidBody.velocity.y);
    }

    void MoveUpdate()
    {
        CheckGroundStatus();

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
            groundCheckDistance = 0f;
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

    void MovementCallback(Vector2 movement)
    {
        _movement = movement;
        if (_movement.magnitude > 1) _movement.Normalize();
        if (_isWalking) _movement *= 0.5f;
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
