using UnityEngine;
using System.Collections;

public class ResetTransformOnEnable : MonoBehaviour
{
    Vector3 _initialPosition = new Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);
    Quaternion _initialRotation;
    Vector3 _initialScale;

    void Start()
    {
        _initialPosition = transform.localPosition;
        _initialRotation = transform.localRotation;
        _initialScale = transform.localScale;
    }

    void OnEnable()
    {
        if (_initialPosition != new Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity))
        {
            transform.localPosition = _initialPosition;
            transform.localRotation = _initialRotation;
            transform.localScale = _initialScale;
        }
    }
}
