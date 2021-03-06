﻿using UnityEngine;
using System.Collections;

public class SandCastlePlacer : MonoBehaviour
{
    public GameObject _sandCastlePrefab;
    Camera _camera;

    // Use this for initialization
    void Start()
    {
        _camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_sandCastlePrefab == null)
            return;
        
        CreateSandCastle();
    }

    void CreateSandCastle()
    {
        if (Input.GetButtonDown("Main"))
        {
            RaycastHit hit;
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
                GameObject.Instantiate(_sandCastlePrefab, hit.point, Quaternion.identity);
        }
    }
}