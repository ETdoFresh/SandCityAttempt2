﻿using UnityEngine;
using System.Collections;

public class MovementNavMeshRigidBody : MonoBehaviour {

    public float stationaryTurnSpeed = 180;
    public float movingTurnSpeed = 360;
    public float groundCheckDistance = 0.5f;
    public float jumpPower = 5f;
    public float gravityMultiplier = 2f;
    public float animationAverageVelocity = 5.661f;
    public RuntimeAnimatorController animatorController;

    NavMeshAgent _navMeshAgent;
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

    void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();

        _navMeshAgent = GetComponent<NavMeshAgent>();
        _navMeshAgent.updatePosition = false;
        _navMeshAgent.updateRotation = true;

        _initialGroundCheckDistance = groundCheckDistance;
    }

    void OnEnable()
    {
        ControlThirdPersonCamera.Movement += OnMovement;
        ControlThirdPersonCamera.Crouch += OnCrouch;
        ControlThirdPersonCamera.Walk += OnWalk;
        ControlThirdPersonCamera.Jump += OnJump;
    }

    void OnDisable()
    {
        ControlThirdPersonCamera.Movement -= OnMovement;
        ControlThirdPersonCamera.Crouch -= OnCrouch;
        ControlThirdPersonCamera.Walk -= OnWalk;
        ControlThirdPersonCamera.Jump -= OnJump;
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

            // TODO: Enforce max force
            //float maxForce = _navMeshAgent.speed * 5;
            //if (force.magnitude > maxForce)
            //    force = force.normalized * maxForce;

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
            _animator.speed = _navMeshAgent.speed / animationAverageVelocity;
        else
            _animator.speed = 1;
    }

    

    void MoveUpdate(Vector3 move)
    {
        if (move.magnitude > 1f) move.Normalize();
        if (_isWalking) move *= 0.5f;
        move = transform.InverseTransformDirection(move);
        
        // TODO: Figure normals, maybe later?
        //move = Vector3.ProjectOnPlane(move, _groundNormal);

        _turnAmount = Mathf.Atan2(move.x, move.z);
        _turnAmount = Mathf.Clamp(_turnAmount, 0, 1);
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
            //_groundNormal = hitInfo.normal;
        }
        else
        {
            _isGrounded = false;
            _animator.applyRootMotion = false;
            //_groundNormal = Vector3.up;
        }
    }

    void ApplyExtraTurnRotation()
    {
        // help the character turn faster (this is in addition to root rotation in the animation)
        float turnSpeed = Mathf.Lerp(stationaryTurnSpeed, movingTurnSpeed, _forwardAmount);
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
