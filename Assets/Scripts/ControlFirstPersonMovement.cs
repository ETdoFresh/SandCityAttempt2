using UnityEngine;
using System.Collections;
using System;

public class ControlFirstPersonMovement : MonoBehaviour
{

    public static event MovementHandler OnMovement;
    public static event CrouchHandler OnCrouch;
    public static event WalkHandler OnWalk;
    public static event JumpHandler OnJump;
    public static event LookAroundHandler OnLookAround;
    public static event Action Enable;
    public static event Action Disable;

    public delegate void MovementHandler(Vector2 movement);
    public delegate void CrouchHandler(bool isCrouching);
    public delegate void WalkHandler(bool isWalking);
    public delegate void JumpHandler(bool isJumping);
    public delegate void LookAroundHandler(float xAmount, float turnAmount);

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
            if (Input.GetButton("Vertical") || Input.GetButton("Horizontal"))
            {
                Vector2 movement = Vector2.zero;
                if (Input.GetButton("Vertical"))
                    movement.y += Input.GetAxis("Vertical");
                if (Input.GetButton("Horizontal"))
                    movement.x += Input.GetAxis("Horizontal");

                OnMovement(movement);
            }
        }
        if (OnLookAround != null)
        {
            if (Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.Q))
            {
                float xAmount = 0;
                float turnAmount = 0;
                if (Input.GetKey(KeyCode.E))
                {
                    xAmount += 0.3f;
                    turnAmount += 1.5f;
                }
                else if (Input.GetKey(KeyCode.Q))
                {
                    xAmount -= 0.3f;
                    turnAmount -= 1.5f;
                }
                OnLookAround(xAmount, turnAmount);
            }
        }

        if (OnCrouch != null) OnCrouch(Input.GetButton("Crouch"));
        if (OnWalk != null) OnWalk(Input.GetButton("Walk"));
        if (OnJump != null) OnJump(Input.GetButtonDown("Jump"));
    }
}
