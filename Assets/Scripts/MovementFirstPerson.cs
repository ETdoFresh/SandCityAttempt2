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

        _initialGroundCheckDistance = groundCheckDistance;
    }

    void OnEnable()
    {
        ControlFirstPersonMovement.Movement += OnMovement;
        ControlFirstPersonMovement.Crouch += OnCrouch;
        ControlFirstPersonMovement.Walk += OnWalk;
        ControlFirstPersonMovement.Jump += OnJump;
    }

    void OnDisable()
    {
        ControlFirstPersonMovement.Movement -= OnMovement;
        ControlFirstPersonMovement.Crouch -= OnCrouch;
        ControlFirstPersonMovement.Walk -= OnWalk;
        ControlFirstPersonMovement.Jump -= OnJump;
    }

    void Update()
    {
        MoveUpdate();
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
            _rigidBody.AddForce(force);
        }
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
        if (_isWalking) _movement *= 0.5f;
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
        groundCheckDistance = _rigidBody.velocity.y < 0 ? _initialGroundCheckDistance : 0.1f;
    }

    void HandleGroundedMovement()
    {
        if (_isJumping && !_isCrouching && _isGrounded)
        {
            _rigidBody.AddForce(new Vector3(0, jumpPower, 0), ForceMode.Impulse);
            _isGrounded = false;
            _animator.applyRootMotion = false;
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
        if (Physics.CheckSphere(_sphereCheckCenter, _radius, layerMask))
        {
            _isGrounded = true;
            _animator.applyRootMotion = true;
        }
        else
        {
            _isGrounded = false;
            _animator.applyRootMotion = false;
        }
    }

    void OnMovement(Vector2 movement)
    {
        _movement = movement;
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
