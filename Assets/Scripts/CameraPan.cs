using UnityEngine;
using System.Collections;

public class CameraPan : MonoBehaviour
{
    public float sensitivity = 1;

    Vector3 _velocity;
    float _acceleration = 5;
    Vector3 _startDrag;
    Vector3 _targetVelocity;
    GameObject _player;

    // Update is called once per frame
    void Update()
    {
        if (_player == null)
            _player = GetPlayer(transform);
        
        if (_player == null)
            return;

        _player.GetComponent<UnityEngine.AI.NavMeshAgent>().SetDestination(_player.transform.position);
        ComputeTargetVelocity();
        ComputeActualVelocity();
        _player.transform.position += _velocity;
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

    void ComputeTargetVelocity()
    {
        if (Input.GetMouseButton(2))
        {
            if (Input.GetMouseButtonDown(2))
                _startDrag = Input.mousePosition;

            _targetVelocity = (Input.mousePosition - _startDrag) * sensitivity;
            _targetVelocity.z = _targetVelocity.y;
            _targetVelocity.y = 0;
        }
        else
            _targetVelocity = Vector3.zero;
    }

    void ComputeActualVelocity()
    {
        Vector3 deltaVelocity = _targetVelocity - _velocity;
        if (deltaVelocity.magnitude > _acceleration)
            _velocity += deltaVelocity.normalized * _acceleration;
        else
            _velocity = _targetVelocity;
    }
}
