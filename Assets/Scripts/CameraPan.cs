using UnityEngine;
using System.Collections;

public class CameraPan : MonoBehaviour
{
    [SerializeField] Vector3 _position;
    [SerializeField] Vector3 _velocity;
    [SerializeField] float _acceleration = 5;
    [SerializeField] Vector3 _startDrag;
    [SerializeField] Vector3 _targetVelocity;
    [SerializeField] float _sensitivity = 1;
    Vector3 _initialOffset;

    // Use this for initialization
    void Start()
    {
        _initialOffset = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Fire2"))
        {
            if (Input.GetButtonDown("Fire2"))
                _startDrag = Input.mousePosition;

            _targetVelocity = (Input.mousePosition - _startDrag) * _sensitivity;
            _targetVelocity.z = _targetVelocity.y;
            _targetVelocity.y = 0;
        }
        else
            _targetVelocity = Vector3.zero;

        Vector3 deltaVelocity = _targetVelocity - _velocity;
        if (deltaVelocity.magnitude > _acceleration)
            _velocity += deltaVelocity.normalized * _acceleration;
        else
            _velocity = _targetVelocity;

        _position += _velocity;

        transform.localPosition = _position + _initialOffset;
    }
}
