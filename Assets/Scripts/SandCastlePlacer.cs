using UnityEngine;
using System.Collections;

public class SandCastlePlacer : MonoBehaviour
{
    [SerializeField] GameObject _sandCastlePrefab;
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

        if (Input.GetButtonDown("Fire1"))
        {
            RaycastHit hit;
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
                GameObject.Instantiate(_sandCastlePrefab, hit.point, Quaternion.identity);
        }
    }
}
