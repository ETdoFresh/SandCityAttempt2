using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraToggle : MonoBehaviour
{
    [SerializeField] List<Camera> _cameras = new List<Camera>();
    [SerializeField] int _currentIndex;
    [SerializeField] Camera _currentCamera;

    // Use this for initialization
    void Start()
    {
        _cameras.AddRange(GameObject.FindObjectsOfType<Camera>());
        _currentCamera = Camera.main;
        
        for (int i = 0; i < _cameras.Count; i++)
            if (_cameras[i] == _currentCamera)
                _currentIndex = i;

        EnableOnlyOneCamera();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            _currentIndex = (_currentIndex + 1) % _cameras.Count;
            _currentCamera = _cameras[_currentIndex];
            EnableOnlyOneCamera();
        }
    }

    private void EnableOnlyOneCamera()
    {
        _currentCamera.gameObject.SetActive(true);

        foreach (Camera camera in _cameras)
            if (camera != _currentCamera)
                camera.gameObject.SetActive(false);
    }
}
