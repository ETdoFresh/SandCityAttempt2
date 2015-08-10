using UnityEngine;
using System.Collections;

public class ControlFirstPersonMovement : MonoBehaviour
{

    public static event MovementHandler MovementEvent;
    public static event CrouchHandler CrouchEvent;
    public static event WalkHandler WalkEvent;
    public static event JumpHandler JumpEvent;

    public delegate void MovementHandler(Camera camera, Vector3 movement);
    public delegate void CrouchHandler(bool isCrouching);
    public delegate void WalkHandler(bool isWalking);
    public delegate void JumpHandler(bool isJumping);

    public Camera _camera;

    void Start()
    {
        _camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 movement = Vector3.zero;
        if (Input.GetButton("Vertical"))
            movement.z += Input.GetAxis("Vertical");
        if (Input.GetButton("Horizontal"))
            movement.x += Input.GetAxis("Horizontal");

        MovementEvent(_camera, movement);
    }
}
