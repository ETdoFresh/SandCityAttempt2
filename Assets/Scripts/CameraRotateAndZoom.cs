using UnityEngine;
using System.Collections;

public class CameraRotateAndZoom : MonoBehaviour
{
    [SerializeField] float _rotation;
    [SerializeField] CameraFollowTarget _cameraFollowTarget;
    [SerializeField] float _zoomSensitivity = 1;

    Vector3 _centerCircle;
    float _cameraDistanceFromCenter;
    float _initialFieldOfView;
    Camera _camera;

    // Use this for initialization
    void Start()
    {
        _camera = GetComponent<Camera>();
        _initialFieldOfView = _camera.fieldOfView;
        _cameraFollowTarget = GetComponent<CameraFollowTarget>();
    }

    // Update is called once per frame
    void Update()
    {
        _centerCircle = new Vector3(_cameraFollowTarget.TargetTracker.x, transform.position.y, _cameraFollowTarget.TargetTracker.z);
        _cameraDistanceFromCenter = Vector3.Distance(transform.position, _centerCircle);

        _rotation = _rotation % 360;
        _rotation = _rotation < 0 ? 360 + _rotation : _rotation;
        float newX = Mathf.Cos(Mathf.Deg2Rad * _rotation);
        float newZ = Mathf.Sin(Mathf.Deg2Rad * _rotation);
        transform.position = new Vector3(_cameraDistanceFromCenter * newX + _centerCircle.x, transform.position.y, _cameraDistanceFromCenter * newZ + _centerCircle.z);
        Vector3 lookAtVector = _cameraFollowTarget.TargetTracker - transform.position;
        transform.rotation = Quaternion.LookRotation(lookAtVector);

        if (Input.GetButton("Vertical"))
        {
            _camera.fieldOfView -= Input.GetAxis("Vertical") * _zoomSensitivity;
            _camera.fieldOfView = Mathf.Clamp(_camera.fieldOfView, 10, 120);
        }

        if (Input.GetButton("Horizontal"))
            _rotation += Input.GetAxis("Horizontal");
    }
}
