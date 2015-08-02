using UnityEngine;
using System.Collections;

public class MainCameraControls : MonoBehaviour
{
    public static event MovementHandler MovementEvent;
    public static event CrouchHandler CrouchEvent;
    public static event WalkHandler WalkEvent;
    public static event JumpHandler JumpEvent;

    public delegate void MovementHandler(Camera camera, Vector3 mousePosition);
    public delegate void CrouchHandler(bool isCrouching);
    public delegate void WalkHandler(bool isWalking);
    public delegate void JumpHandler(bool isJumping);

    private Camera _camera;

    void Start()
    {
        _camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Fire1"))
            MovementEvent(_camera, Input.mousePosition);

        CrouchEvent(Input.GetKey(KeyCode.C));
        WalkEvent(Input.GetButton("Fire3"));
        JumpEvent(Input.GetButtonDown("Jump"));
    }
}
