using UnityEngine;
using System.Collections;
using System;

public class ControlThirdPersonCamera : MonoBehaviour
{
    public static event MovementHandler OnMovement;
    public static event CrouchHandler OnCrouch;
    public static event WalkHandler OnWalk;
    public static event JumpHandler OnJump;
    public static event Action Enable;
    public static event Action Disable;

    public delegate void MovementHandler(Camera camera, Vector3 mousePosition);
    public delegate void CrouchHandler(bool isCrouching);
    public delegate void WalkHandler(bool isWalking);
    public delegate void JumpHandler(bool isJumping);

    Camera _camera;

    void Start()
    {
        _camera = GetComponent<Camera>();
    }

    void OnEnable()
    {
        if (Enable != null) Enable();
    }

    void OnDisable()
    {
        if (Disable != null) Disable();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Main"))
            if (OnMovement != null) OnMovement(_camera, Input.mousePosition);

        if (OnCrouch != null) OnCrouch(Input.GetButton("Crouch"));
        if (OnWalk != null) OnWalk(Input.GetButton("Walk"));
        if (OnJump != null) OnJump(Input.GetButtonDown("Jump"));
    }
}
