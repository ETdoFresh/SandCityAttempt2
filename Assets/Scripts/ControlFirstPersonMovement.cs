using UnityEngine;
using System.Collections;
using System;

public class ControlFirstPersonMovement : MonoBehaviour
{

    public static event MovementHandler Movement;
    public static event CrouchHandler Crouch;
    public static event WalkHandler Walk;
    public static event JumpHandler Jump;
    public static event Action Enable;
    public static event Action Disable;

    public delegate void MovementHandler(Vector2 movement);
    public delegate void CrouchHandler(bool isCrouching);
    public delegate void WalkHandler(bool isWalking);
    public delegate void JumpHandler(bool isJumping);

    void OnEnable()
    {
        if (Enable != null) Enable();
    }

    void OnDisable()
    {
        if (Disable != null) Disable();
    }

    void Update()
    {
        if (Movement != null)
        {
            Vector2 movement = Vector2.zero;
            if (Input.GetButton("Vertical"))
                movement.y += Input.GetAxis("Vertical");
            if (Input.GetButton("Horizontal"))
                movement.x += Input.GetAxis("Horizontal");
            Movement(movement);
        }
        if (Crouch != null) Crouch(Input.GetButton("Crouch"));
        if (Walk != null) Walk(Input.GetButton("Walk"));
        if (Jump != null) Jump(Input.GetButtonDown("Jump"));
    }
}
