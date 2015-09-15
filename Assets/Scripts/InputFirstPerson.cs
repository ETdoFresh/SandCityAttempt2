using UnityEngine;
using System.Collections;
using System;

public class InputFirstPerson : MonoBehaviour
{
    public static event ZoomHandler OnZoom;
    public static event SandActionHandler OnSandAction;
    public static event SandActionCompleteHandler OnSandActionComplete;
    public static event Action Enable;
    public static event Action Disable;

    public delegate void ZoomHandler(float amount);
    public delegate void SandActionHandler(Vector3 position);
    public delegate void SandActionCompleteHandler(Vector3 position);

    public float zoomSensitivity = 1;
    Camera _camera;
    Vector3 _lastMousePosition;

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
        if (Input.GetButtonDown("Main") || (Input.GetButton("Main") && Input.mousePosition != _lastMousePosition))
        {
            RaycastHit hit;
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
                OnSandAction(hit.point);

            _lastMousePosition = Input.mousePosition;
        }
        if (Input.GetButtonUp("Main"))
        {
            RaycastHit hit;
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
                OnSandActionComplete(hit.point);
        }
    }
}
