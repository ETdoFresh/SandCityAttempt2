using UnityEngine;
using System.Collections;

public class TerrainDig : MonoBehaviour
{
    public delegate void SandDigHandler(int amountOfSand);
    public delegate int TakeSandHandler(int amountOfSand);
    public event SandDigHandler SandDig;
    public event TakeSandHandler TakeSand;
    public float digRate = 0.001f;

    Terrain _terrain;
    Vector3 _lastPosition;
    Camera _camera;

    // Use this for initialization
    void Start()
    {
        _camera = GetComponent<Camera>();
        _terrain = GameObject.FindObjectOfType<Terrain>();
        _lastPosition = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Fire1") && _lastPosition != Input.mousePosition)
        {
            _lastPosition = Input.mousePosition;
            DigInSand();
        }
        else if (Input.GetButton("Fire2") && _lastPosition != Input.mousePosition)
        {
            _lastPosition = Input.mousePosition;
            PlaceSand();
        }
        else if (Input.GetButtonUp("Fire1"))
            _lastPosition = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
    }

    void PlaceSand()
    {
        RaycastHit hit;
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            int takenSand = 0;
            if (TakeSand != null)
                takenSand = TakeSand(16);

            if (takenSand == 0)
                return;

            // TODO: If takenSand < 16, only place takenSand

            Vector2 pointOnTerrain = new Vector2(hit.point.x + 25, hit.point.z + 25);
            int width = _terrain.terrainData.heightmapWidth;
            int height = _terrain.terrainData.heightmapHeight;
            int xBase = (int)(pointOnTerrain.x / 50 * width);
            int yBase = (int)(pointOnTerrain.y / 50 * height);

            float[,] heights = _terrain.terrainData.GetHeights(xBase - 2, yBase - 2, 4, 4);

            for (int i = 0; i < heights.GetLength(0); i++)
                for (int j = 0; j < heights.GetLength(0); j++)
                    heights[i, j] = Mathf.Max(0, heights[i, j] + digRate);

            _terrain.terrainData.SetHeights(xBase - 2, yBase - 2, heights);
        }
    }

    void DigInSand()
    {
        RaycastHit hit;
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            if (SandDig != null)
                SandDig(16);

            Vector2 pointOnTerrain = new Vector2(hit.point.x + 25, hit.point.z + 25);
            int width = _terrain.terrainData.heightmapWidth;
            int height = _terrain.terrainData.heightmapHeight;
            int xBase = (int)(pointOnTerrain.x / 50 * width);
            int yBase = (int)(pointOnTerrain.y / 50 * height);

            float[,] heights = _terrain.terrainData.GetHeights(xBase - 2, yBase - 2, 4, 4);

            for (int i = 0; i < heights.GetLength(0); i++)
                for (int j = 0; j < heights.GetLength(0); j++)
                    heights[i, j] = Mathf.Max(0, heights[i, j] - digRate);

            _terrain.terrainData.SetHeights(xBase - 2, yBase - 2, heights);
        }
    }

    void OnDestroy()
    {
        SandDig = null;
    }
}
