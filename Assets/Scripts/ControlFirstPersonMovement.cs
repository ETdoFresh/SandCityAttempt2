﻿using UnityEngine;
using System.Collections;
using System;

public class ControlFirstPersonMovement : MonoBehaviour
{

    public static event MovementHandler OnMovement;
    public static event CrouchHandler OnCrouch;
    public static event WalkHandler OnWalk;
    public static event JumpHandler OnJump;
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
        if (OnMovement != null)
        {
            Vector2 movement = Vector2.zero;
            if (Input.GetButton("Vertical"))
                movement.y += Input.GetAxis("Vertical");
            if (Input.GetButton("Horizontal"))
                movement.x += Input.GetAxis("Horizontal");
            OnMovement(movement);
        }
        if (OnCrouch != null) OnCrouch(Input.GetButton("Crouch"));
        if (OnWalk != null) OnWalk(Input.GetButton("Walk"));
        if (OnJump != null) OnJump(Input.GetButtonDown("Jump"));
    }
}
