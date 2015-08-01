using UnityEngine;
using System.Collections;

public class CameraFollowTarget : MonoBehaviour
{
    [SerializeField] Transform _target;
    [SerializeField] float _maxDistanceFromTarget = 0;
    Vector3 _targetTracker;

    // Use this for initialization
    void Start()
    {
        if (_target != null)
            _targetTracker = _target.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (_target != null)
        {
            Vector3 deltaPosition = _target.position - _targetTracker;

            if (deltaPosition.magnitude > _maxDistanceFromTarget)
            {
                float desiredTrackerMovement = deltaPosition.magnitude - _maxDistanceFromTarget;
                _targetTracker += desiredTrackerMovement * deltaPosition.normalized;
                transform.position += desiredTrackerMovement * deltaPosition.normalized;
            }
        }
    }
}
