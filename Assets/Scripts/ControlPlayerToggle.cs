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
    public CharacterFootStep _characterFootStep;

    // Use this for initialization
    void Start()
    {
        _animator = GetComponent<Animator>();
        _thirdPersonController = GetComponent<MovementNavMeshRigidBody>();
        _firstPersonController = GetComponent<MovementFirstPerson>();
        _characterFootStep = GetComponentInChildren<CharacterFootStep>();

        _thirdPersonController.enabled = true;
        _firstPersonController.enabled = false;
        _characterFootStep.enabled = true;
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
        if (_animator != null) _animator.runtimeAnimatorController = firstPersonAnimatorController;
        if (_firstPersonController != null) _firstPersonController.enabled = true;
        if (_thirdPersonController != null) _thirdPersonController.enabled = false;
        if (_characterFootStep != null) _characterFootStep.enabled = false;
    }

    void EnableThirdPersonController()
    {
        _animator.runtimeAnimatorController = thirdPersonAnimatorController;
        _thirdPersonController.enabled = true;
        _characterFootStep.enabled = true;
        _firstPersonController.enabled = false;
    }
}
