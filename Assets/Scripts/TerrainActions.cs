using UnityEngine;
using System.Collections;
using System;

public class TerrainActions : MonoBehaviour
{
    public float digRate = 0.001f;
    public int digSize = 2;

    Terrain _terrain;
    public Vector2 _lastPosition;

    // TODO: Can be local variables
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
        //float threshold = 0.1f;
        //if (Vector2.Distance(_lastPosition, pointOnTerrain) <= threshold)
        //    return;
        if (_lastPosition == Vector2.zero)
        {
            _lastPosition = pointOnTerrain;
            return;
        }

        Vector2 pointOnHeightmap, lastPointOnHeightmap;
        pointOnHeightmap.x = pointOnTerrain.x / _terrain.terrainData.size.x * _terrain.terrainData.heightmapWidth;
        pointOnHeightmap.y = pointOnTerrain.y / _terrain.terrainData.size.z * _terrain.terrainData.heightmapHeight;
        lastPointOnHeightmap.x = _lastPosition.x / _terrain.terrainData.size.x * _terrain.terrainData.heightmapWidth;
        lastPointOnHeightmap.y = _lastPosition.y / _terrain.terrainData.size.z * _terrain.terrainData.heightmapHeight;
        _lastPosition = pointOnTerrain;
        float radius = digSize / 2f;
        int halfIntDigSize = (int)(Mathf.Floor(radius));
        Vector2 min, max;
        min.x = Mathf.Min(lastPointOnHeightmap.x, pointOnHeightmap.x) - radius;
        min.y = Mathf.Min(lastPointOnHeightmap.y, pointOnHeightmap.y) - radius;
        max.x = Mathf.Max(lastPointOnHeightmap.x, pointOnHeightmap.x) + radius;
        max.y = Mathf.Max(lastPointOnHeightmap.y, pointOnHeightmap.y) + radius;
        int x = (int)Mathf.Floor(min.x);
        int y = (int)Mathf.Floor(min.y);
        int width = (int)Mathf.Round(max.x - min.x);
        int height = (int)Mathf.Round(max.y - min.y);
        float[,] heights = _terrain.terrainData.GetHeights(x, y, width, height);
        int heightsWidth = heights.GetLength(0);
        int heightsHeight = heights.GetLength(1);

        Vector2 center1 = new Vector2(lastPointOnHeightmap.y - min.y, lastPointOnHeightmap.x - min.x);
        Vector2 center2 = new Vector2(pointOnHeightmap.y - min.y, pointOnHeightmap.x - min.x);
        Debug.Log(center2);

        int[,] mask = new int[heightsWidth, heightsHeight];
        if (digSize > 1)
        {
            for (int i = 0; i < heightsWidth; i++)
            {
                for (int j = 0; j < heightsHeight; j++)
                {
                    if (Vector2.Distance(center1, new Vector2(i, j)) <= radius)
                    {
                        mask[i, j] = 1;
                    }
                    if (Vector2.Distance(center2, new Vector2(i, j)) <= radius)
                    {
                        mask[i, j] = 1;
                    }
                    Debug.Log(i +","+j+": "+GetLineDistance(center1, center2, new Vector2(i, j)));
                    if (GetLineDistance(center1, center2, new Vector2(i, j)) <= radius)
                    {
                        mask[i, j] = 1;
                    }
                }
            }
        }
        else if (digSize == 1)
            mask[0, 0] = 1;

        for (int i = 0; i < heightsWidth; i++)
            for (int j = 0; j < heightsHeight; j++)
                heights[i, j] = Mathf.Max(0, heights[i, j] - digRate * mask[i, j]);

        _terrain.terrainData.SetHeights(x, y, heights);

        // TODO: Cover edge cases, literally
    }

    float GetLineDistance(Vector2 center1, Vector2 center2, Vector2 point)
    {
        Vector2 delta;
        delta.x = center2.x - center1.x;
        delta.y = center1.y - center2.y;
        float numerator = Mathf.Abs(delta.x * point.y - delta.y * point.x + center2.y * center1.x - center1.y * center2.x);
        float denomentator = Mathf.Sqrt(Mathf.Pow(delta.y, 2) + Mathf.Pow(delta.x, 2));
        return numerator / denomentator;
    }
}
