using UnityEngine;
using System.Collections;

public class ControlTerrainDig : MonoBehaviour
{
    public delegate void DigInSandHandler(int amountOfSand);
    public delegate int AddSandHandler(int amountOfSand);
    public event DigInSandHandler OnDig;
    public event AddSandHandler OnPlace;
    public float digRate = 0.001f;
    public int digSize = 4;

    enum ModifyDirection { UP, DOWN }
    Camera _camera;
    Terrain _terrain;
    Vector3 _lastPosition;
    
    public int width;
    public int height;
    public int xBase;
    public int yBase;
    public Vector2 pointOnTerrain;

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
        if (Input.GetButton("Main") && _lastPosition != Input.mousePosition)
        {
            _lastPosition = Input.mousePosition;
            ModifySandDown();
        }
        else if (Input.GetButton("Alternate") && _lastPosition != Input.mousePosition)
        {
            _lastPosition = Input.mousePosition;
            ModifySandUp();
        }
        else if (Input.GetButtonUp("Main"))
            _lastPosition = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
    }

    void OnDestroy()
    {
        OnDig = null;
    }

    void ModifySandUp()
    {
        ModifySand(ModifyDirection.UP);
    }

    void ModifySandDown()
    {
        ModifySand(ModifyDirection.DOWN);
    }

    void ModifySand(ModifyDirection modifyDirection)
    {
        int modifySign = modifyDirection == ModifyDirection.DOWN ? -1 : 1;
        RaycastHit hit;
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            if (OnDig != null && modifyDirection == ModifyDirection.DOWN)
                OnDig(16);
            else if (OnPlace != null && modifyDirection == ModifyDirection.UP)
                if (OnPlace(16) <= 0)
                    return;

            pointOnTerrain = new Vector2(hit.point.x - _terrain.gameObject.transform.position.x, hit.point.z - _terrain.gameObject.transform.position.z);
            width = _terrain.terrainData.heightmapWidth;
            height = _terrain.terrainData.heightmapHeight;
            xBase = (int)(pointOnTerrain.x / _terrain.terrainData.size.x * width);
            yBase = (int)(pointOnTerrain.y / _terrain.terrainData.size.z * height);

            float[,] heights = _terrain.terrainData.GetHeights((int)(xBase - digSize / 2f), (int)(yBase - digSize / 2f), digSize, digSize);

            for (int i = 0; i < heights.GetLength(0); i++)
                for (int j = 0; j < heights.GetLength(0); j++)
                    heights[i, j] = Mathf.Max(0, heights[i, j] + digRate * modifySign);

            _terrain.terrainData.SetHeights(xBase - digSize / 2, yBase - digSize / 2, heights);

            // TODO: Cover edge cases, literally
        }
    }
}
