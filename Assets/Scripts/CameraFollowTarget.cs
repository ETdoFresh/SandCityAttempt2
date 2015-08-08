using UnityEngine;
using System.Collections;

public class CameraFollowTarget : MonoBehaviour
{
    public Transform target;
    public float maxDistanceFromTarget = 0;
    Vector3 _targetTracker;

    // Use this for initialization
    void Start()
    {
        if (target != null)
            _targetTracker = target.position;
    }

    // Update is called once per frame
    void Update()
    {
        MoveWithinMaxDistanceOfTarget();
    }

    void MoveWithinMaxDistanceOfTarget()
    {
        if (target != null)
        {
            Vector3 deltaPosition = target.position - _targetTracker;

            if (deltaPosition.magnitude > maxDistanceFromTarget)
            {
                float desiredTrackerMovement = deltaPosition.magnitude - maxDistanceFromTarget;
                _targetTracker += desiredTrackerMovement * deltaPosition.normalized;
                transform.position += desiredTrackerMovement * deltaPosition.normalized;
            }
        }
    }

    public Vector3 TargetTracker { get { return _targetTracker; } }
}
