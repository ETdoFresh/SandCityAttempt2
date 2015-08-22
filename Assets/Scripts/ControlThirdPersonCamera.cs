using UnityEngine;
using System.Collections;
using System;

public class ControlThirdPersonCamera : MonoBehaviour
{
    public static event MovementHandler Movement;
    public static event CrouchHandler Crouch;
    public static event WalkHandler Walk;
    public static event JumpHandler Jump;
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
            if (Movement != null) Movement(_camera, Input.mousePosition);

        if (Crouch != null) Crouch(Input.GetButton("Crouch"));
        if (Walk != null) Walk(Input.GetButton("Walk"));
        if (Jump != null) Jump(Input.GetButtonDown("Jump"));
    }
}
