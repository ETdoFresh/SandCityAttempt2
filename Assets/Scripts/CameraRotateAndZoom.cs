using UnityEngine;
using System.Collections;

public class CameraRotateAndZoom : MonoBehaviour
{
    public float rotation;
    public float zoomSensitivity = 1;

    Camera _camera;
    //float _initialFieldOfView;
    CameraFollowTarget _cameraFollowTarget;
    Vector3 _centerCircle;
    float _cameraDistanceFromCenter;
    private Vector3 _mousePostionDown;

    void Start()
    {
        _camera = GetComponent<Camera>();
        //_initialFieldOfView = _camera.fieldOfView;
        _cameraFollowTarget = GetComponent<CameraFollowTarget>();
    }

    void Update()
    {
        _centerCircle = new Vector3(_cameraFollowTarget.TargetTracker.x, transform.position.y, _cameraFollowTarget.TargetTracker.z);
        _cameraDistanceFromCenter = Vector3.Distance(transform.position, _centerCircle);
        NormalizeRotation();
        MoveAndRotateCamera();
        ZoomCamera();
        RotateCamera();
    }

    void NormalizeRotation()
    {
        rotation = rotation % 360;
        rotation = rotation < 0 ? 360 + rotation : rotation;
    }

    void MoveAndRotateCamera()
    {
        float newX = Mathf.Cos(Mathf.Deg2Rad * rotation);
        float newZ = Mathf.Sin(Mathf.Deg2Rad * rotation);
        transform.position = new Vector3(_cameraDistanceFromCenter * newX + _centerCircle.x, transform.position.y, _cameraDistanceFromCenter * newZ + _centerCircle.z);
        Vector3 lookAtVector = _cameraFollowTarget.TargetTracker - transform.position;
        transform.rotation = Quaternion.LookRotation(lookAtVector);
    }

    void ZoomCamera()
    {
        if (Input.GetButton("Vertical") || Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            if (Input.GetButton("Vertical"))
                _camera.fieldOfView -= Input.GetAxis("Vertical") * zoomSensitivity;
            else
                _camera.fieldOfView -= Input.GetAxis("Mouse ScrollWheel") * 10 * zoomSensitivity;

            _camera.fieldOfView = Mathf.Clamp(_camera.fieldOfView, 10, 120);
        }
    }

    void RotateCamera()
    {
        if (Input.GetButton("Horizontal"))
            rotation += Input.GetAxis("Horizontal");
        else if (Input.GetButton("Alternate"))
        {
            if (Input.GetButtonDown("Alternate"))
                _mousePostionDown = Input.mousePosition;

            float deltaXPosition = Input.mousePosition.x - _mousePostionDown.x;
            int direction = (int)(Mathf.Abs(deltaXPosition) / deltaXPosition);

            rotation += direction * Vector3.Distance(_mousePostionDown, Input.mousePosition) * Time.deltaTime;
        }

    }
}
