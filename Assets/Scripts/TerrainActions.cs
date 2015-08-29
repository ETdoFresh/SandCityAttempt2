using UnityEngine;
using System.Collections;

public class TerrainActions : MonoBehaviour
{
    public float digRate = 0.001f;
    public int digSize = 2;

    Terrain _terrain;
    public Vector2 _lastPosition;

    // TODO: Can be local variables
    public int width;
    public int height;
    public int xBase;
    public int yBase;
    public Vector2 pointOnTerrain;

    void Start()
    {
        _terrain = GetComponent<Terrain>();
    }

    void OnEnable()
    {
        PlayerActions.OnSandFinger += WriteInSand;
    }

    void OnDisable()
    {
        PlayerActions.OnSandFinger -= WriteInSand;
    }

    void WriteInSand(Vector3 here)
    {
        FingerDraw(here);
    }

    void FingerDraw(Vector3 here)
    {
        pointOnTerrain = new Vector2(here.x - _terrain.gameObject.transform.position.x, here.z - _terrain.gameObject.transform.position.z);
        float threshold = 0.1f;
        if (Vector2.Distance(_lastPosition, pointOnTerrain) <= threshold)
            return;

        _lastPosition = pointOnTerrain;
        width = _terrain.terrainData.heightmapWidth;
        height = _terrain.terrainData.heightmapHeight;
        xBase = (int)(pointOnTerrain.x / _terrain.terrainData.size.x * width);
        yBase = (int)(pointOnTerrain.y / _terrain.terrainData.size.z * height);

        float radius = digSize / 2f;
        int halfIntDigSize = (int)(Mathf.Floor(radius));
        float[,] heights = _terrain.terrainData.GetHeights(xBase - halfIntDigSize, yBase - halfIntDigSize, digSize, digSize);
        int heightsWidth = heights.GetLength(0);
        int heightsHeight = heights.GetLength(1);

        Vector2 center = new Vector2(radius, radius);
        int[,] mask = new int[heightsWidth, heightsHeight];

        if (digSize > 1)
        {
            for (int i = 0; i < heightsWidth; i++)
                for (int j = 0; j < heightsHeight; j++)
                    if (Vector2.Distance(center, new Vector2(i, j)) <= radius)
                        mask[i, j] = 1;
        }
        else if (digSize == 1)
            mask[0, 0] = 1;

            for (int i = 0; i < heightsWidth; i++)
                for (int j = 0; j < heightsHeight; j++)
                    heights[i, j] = Mathf.Max(0, heights[i, j] - digRate * mask[i, j]);

        _terrain.terrainData.SetHeights(xBase - halfIntDigSize, yBase - halfIntDigSize, heights);

        // TODO: Cover edge cases, literally
    }
}
