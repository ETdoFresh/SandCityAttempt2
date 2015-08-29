using UnityEngine;
using System.Collections;
using System;

public class InputFirstPerson : MonoBehaviour
{
    public static event ZoomHandler OnZoom;
    public static event SandActionHandler OnSandAction;
    public static event Action Enable;
    public static event Action Disable;

    public delegate void ZoomHandler(float amount);
    public delegate void SandActionHandler(Vector3 position);

    public float zoomSensitivity = 1;
    Camera _camera;

    void Start()
    {
        _camera = GetComponent<Camera>();
    }

    void OnEnable()
    {
        if (Enable != null) Enable();
    }

    void OnDisable()
    {
        if (Disable != null) Disable();
    }

    void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
            OnZoom(Input.GetAxis("Mouse ScrollWheel") * zoomSensitivity);
        if (Input.GetButtonDown("Main"))
        {
            RaycastHit hit;
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
                OnSandAction(hit.point);
        }
    }
}
