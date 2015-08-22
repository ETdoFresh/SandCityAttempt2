using UnityEngine;
using System.Collections;
using System;

public class ControlPlayerToggle : MonoBehaviour
{
    public RuntimeAnimatorController firstPersonAnimatorController;
    public RuntimeAnimatorController thirdPersonAnimatorController;

    public Animator _animator;
    public MovementNavMeshRigidBody _thirdPersonController;
    public MovementFirstPerson _firstPersonController;

    // Use this for initialization
    void Start()
    {
        _animator = GetComponent<Animator>();
        _thirdPersonController = GetComponent<MovementNavMeshRigidBody>();
        _firstPersonController = GetComponent<MovementFirstPerson>();
        //_thirdPersonController.enabled = false;
        _firstPersonController.enabled = false;
    }

    void OnEnable()
    {
        ControlFirstPersonMovement.Enable += EnableFirstPersonController;
        ControlThirdPersonCamera.Enable += EnableThirdPersonController;
    }

    void OnDisable()
    {
        ControlFirstPersonMovement.Enable -= EnableFirstPersonController;
        ControlThirdPersonCamera.Enable -= EnableThirdPersonController;
    }

    void EnableFirstPersonController()
    {
        _animator.runtimeAnimatorController = firstPersonAnimatorController;
        _firstPersonController.enabled = true;
        _thirdPersonController.enabled = false;
    }

    void EnableThirdPersonController()
    {
        _animator.runtimeAnimatorController = thirdPersonAnimatorController;
        _thirdPersonController.enabled = true;
        _firstPersonController.enabled = false;
    }
}
