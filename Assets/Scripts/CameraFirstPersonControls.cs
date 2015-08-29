using UnityEngine;
using System.Collections;

public class CameraFirstPersonControls : MonoBehaviour
{
    // offset should affect local position
    // rotation should rotate entire character
    // zoom should affect distance of camera to target.

    public Transform target;

    public float _zoom = 1;
    public bool resetOnEnable = true;
    public float minimumZoom = 0.5f;
    public float maximumZoom = 4;
    float _previousZoom;
    float _initialDistance = 1;

    void Start()
    {
        _initialDistance = Vector3.Distance(transform.localPosition, target.localPosition);
        _previousZoom = _zoom;
    }

    void OnEnable ()
    {
        InputFirstPerson.OnZoom += ChangeZoom;
        if (resetOnEnable)
            _zoom = 1;
    }

    void OnDisable()
    {
        InputFirstPerson.OnZoom -= ChangeZoom;
    }

    void Update()
    {
        if (_zoom != _previousZoom)
        {
            _zoom = Mathf.Clamp(_zoom, minimumZoom, maximumZoom);
            Vector3 direction = (transform.localPosition - target.localPosition).normalized;
            transform.localPosition = direction * _initialDistance / _zoom + target.localPosition;
        }
    }

    void ChangeZoom(float amount)
    {
        _zoom += amount;
    }
}
