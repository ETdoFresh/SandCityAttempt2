using UnityEngine;
using System.Collections;

public class MovementRigidBody : MonoBehaviour
{
    public float groundCheckDistance = 0.2f;
    NavMeshAgent _navMeshAgent;
    Rigidbody _rigidBody;
    Animator _animator;
    Vector3 _groundNormal;
    float _initialGroundCheckDistance;
    bool _isGrounded;
    bool _isCrouching;
    bool _isWalking;
    bool _isJumping;
    Vector3 _movement;

    void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();

        _initialGroundCheckDistance = groundCheckDistance;

        ControlFirstPersonMovement.MovementEvent += OnMovement;
        ControlFirstPersonMovement.CrouchEvent += OnCrouch;
        ControlFirstPersonMovement.WalkEvent += OnWalk;
        ControlFirstPersonMovement.JumpEvent += OnJump;
    }

    void OnEnabled()
    {
        _animator.enabled = false;
        _navMeshAgent.enabled = false;
    }

    void OnDestroy()
    {
        ControlFirstPersonMovement.MovementEvent -= OnMovement;
        ControlFirstPersonMovement.CrouchEvent -= OnCrouch;
        ControlFirstPersonMovement.WalkEvent -= OnWalk;
        ControlFirstPersonMovement.JumpEvent -= OnJump;
    }

    void FixedUpdate()
    {
        if (_movement.magnitude > 1)
            _movement.Normalize();

        _rigidBody.AddForce(_movement);
    }

    void CheckGroundStatus()
    {
#if UNITY_EDITOR
        // helper to visualise the ground check ray in the scene view
        Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * groundCheckDistance));
#endif
        // 0.1f is a small offset to start the ray from inside the character
        // it is also good to note that the transform position in the sample assets is at the base of the character
        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, groundCheckDistance))
        {
            _groundNormal = hitInfo.normal;
            _isGrounded = true;
        }
        else
        {
            _isGrounded = false;
            _groundNormal = Vector3.up;
        }
    }

    void OnMovement(Camera camera, Vector3 movement)
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
