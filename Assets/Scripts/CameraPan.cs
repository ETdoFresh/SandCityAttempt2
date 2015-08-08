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
    [SerializeField] GameObject _player;

    // Update is called once per frame
    void Update()
    {
        if (_player == null)
            _player = GetPlayer(transform);
        
        if (_player == null)
            return;

        _position = _player.transform.position;
        _player.GetComponent<NavMeshAgent>().SetDestination(_position);

        if (Input.GetMouseButton(2))
        {
            if (Input.GetMouseButtonDown(2))
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
        _player.transform.position = _position;
    }

    GameObject GetPlayer(Transform childTransform)
    {
        if (childTransform.gameObject.tag == "Player")
            return childTransform.gameObject;
        else if (childTransform.parent == null)
            return null;
        else
            return GetPlayer(childTransform.parent);
    }
}
