using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class TerrainActions : MonoBehaviour
{
    Terrain _terrain;
    public Vector2 _lastPosition;

    public float _reserves;

    void Start()
    {
        _terrain = GetComponent<Terrain>();
    }

    void OnEnable()
    {
        PlayerActions.OnSandFinger += Finger;
        PlayerActions.OnScoop += Scoop;
        PlayerActions.OnScoopFinish += ScoopFinish;
        PlayerActions.OnSmooth += Smooth;
        PlayerActions.OnLiftFinger += LiftFinger;
        CharacterFootStep.OnFootStep += FootStep;
    }

    void OnDisable()
    {
        PlayerActions.OnSandFinger -= Finger;
        PlayerActions.OnScoop -= Scoop;
        PlayerActions.OnScoopFinish -= ScoopFinish;
        PlayerActions.OnSmooth -= Smooth;
        PlayerActions.OnLiftFinger -= LiftFinger;
        CharacterFootStep.OnFootStep -= FootStep;
    }

    void FootStep(Vector2 position, Vector2 size, float digRate)
    {
        Vector2 pointOnTerrain = new Vector2(position.x - _terrain.gameObject.transform.position.x, position.y - _terrain.gameObject.transform.position.z);
        Vector2 pointOnHeightmap = GetPointOnHeightMap(pointOnTerrain);
        Vector2 sizeOnHeightmap = GetSizeOnHeightMap(size);
        int baseX = Mathf.FloorToInt(pointOnHeightmap.x - sizeOnHeightmap.x / 2);
        int baseY = Mathf.FloorToInt(pointOnHeightmap.y - sizeOnHeightmap.y / 2);
        int width = Mathf.CeilToInt(sizeOnHeightmap.x);
        int height = Mathf.CeilToInt(sizeOnHeightmap.y);
        float[,] heights = _terrain.terrainData.GetHeights(baseX, baseY, width, height);
        BrushTool.ApplyRate(heights, -digRate);
        _terrain.terrainData.SetHeights(baseX, baseY, heights);
    }

    void Finger(Vector3 position, int digSize, float digRate)
    {
        Vector2 pointOnTerrain = new Vector2(position.x - _terrain.gameObject.transform.position.x, position.z - _terrain.gameObject.transform.position.z);
        if (IsLastPositionEmpty(pointOnTerrain))
            return;
        if (IsNextActionOutsideThreshold(pointOnTerrain, 0.01f))
            return;

        Vector2 pointOnHeightmap = GetPointOnHeightMap(pointOnTerrain);
        Vector2 lastPointOnHeightmap = GetPointOnHeightMap(_lastPosition);
        float radius = digSize / 2f;
        int radiusInt = Mathf.FloorToInt(radius);

        int baseX = Mathf.FloorToInt(pointOnHeightmap.x - radius);
        int baseY = Mathf.FloorToInt(pointOnHeightmap.y - radius);
        int width = digSize;
        int height = digSize;

        //Vector2 min = new Vector2(pointOnHeightmap.x - radius, pointOnHeightmap.y - radius);
        //Vector2 max = new Vector2(pointOnHeightmap.x + radius, pointOnHeightmap.y + radius);

        //int baseX = Mathf.FloorToInt(min.x);
        //int baseY = Mathf.FloorToInt(min.y);
        //int width = Mathf.CeilToInt(max.x - min.x);
        //int height = Mathf.CeilToInt(max.y - min.y);
        float[,] heights = _terrain.terrainData.GetHeights(baseX, baseY, width, height);
        float[,] mask = BrushTool.GetCircleMask(digSize);
        BrushTool.ApplyMask(heights, mask, -digRate);

        //Vector2 center1 = new Vector2(lastPointOnHeightmap.x - min.x, lastPointOnHeightmap.y - min.y);
        //Vector2 center2 = new Vector2(pointOnHeightmap.x - min.x, pointOnHeightmap.y - min.y);

        //float[,] mask = GetDepressedMask(digSize, radius, width, height, center1, center2);
        //heights = DepressHeightmap(digRate, heights, width, height, mask);
        //float reserves = GetReserves(digRate, mask);
        //mask = GetRaisedMask(digSize, radius, width, height, center1, center2);
        //heights = RaiseHeightmap(heights, width, height, mask, reserves);

        _terrain.terrainData.SetHeights(baseX, baseY, heights);
        _lastPosition = pointOnTerrain;

        // TODO: Cover edge cases, literally
    }

    void Smooth(Vector3 position, int digSize, float digRate)
    {
        
    }

    void Scoop(Vector3 position, int digSize, float digRate)
    {
        Vector2 pointOnTerrain = new Vector2(position.x - _terrain.gameObject.transform.position.x, position.z - _terrain.gameObject.transform.position.z);
        if (IsLastPositionEmpty(pointOnTerrain))
            return;
        if (IsNextActionOutsideThreshold(pointOnTerrain, 0.01f))
            return;

        Vector2 pointOnHeightmap = GetPointOnHeightMap(pointOnTerrain);
        Vector2 lastPointOnHeightmap = GetPointOnHeightMap(_lastPosition);
        float radius = digSize / 2f;
        int radiusInt = (int)(Mathf.Floor(radius));

        Vector2 min = new Vector2(pointOnHeightmap.x - radius, pointOnHeightmap.y - radius);
        Vector2 max = new Vector2(pointOnHeightmap.x + radius, pointOnHeightmap.y + radius);

        int baseX = Mathf.FloorToInt(min.x);
        int baseY = Mathf.FloorToInt(min.y);
        int width = Mathf.CeilToInt(max.x - min.x);
        int height = Mathf.CeilToInt(max.y - min.y);
        float[,] heights = _terrain.terrainData.GetHeights(baseX, baseY, width, height);

        int numberOfColumns = heights.GetLength(0);
        int numberOfRows = heights.GetLength(1);

        Vector2 center1 = new Vector2(lastPointOnHeightmap.x - min.x, lastPointOnHeightmap.y - min.y);
        Vector2 center2 = new Vector2(pointOnHeightmap.x - min.x, pointOnHeightmap.y - min.y);
        float[,] mask = GetDepressedMask(digSize, radius, numberOfColumns, numberOfRows, center1, center2);

        _reserves += GetReserves(digRate, mask);
        heights = DepressHeightmap(digRate, heights, numberOfColumns, numberOfRows, mask);

        _terrain.terrainData.SetHeights(baseX, baseY, heights);
        _lastPosition = pointOnTerrain;

        // TODO: Cover edge cases, literally
    }

    private void ScoopFinish(PlayerActions playerActionsScript, Vector3 position)
    {
        Vector2 pointOnTerrain = new Vector2(position.x - _terrain.gameObject.transform.position.x, position.z - _terrain.gameObject.transform.position.z);
        Vector2 pointOnHeightmap = GetPointOnHeightMap(pointOnTerrain);
        float radius = 5;
        int radiusInt = (int)(Mathf.Floor(radius));

        Vector2 min = new Vector2(pointOnHeightmap.x - radius, pointOnHeightmap.y - radius);
        Vector2 max = new Vector2(pointOnHeightmap.x + radius, pointOnHeightmap.y + radius);

        int baseX = Mathf.FloorToInt(min.x);
        int baseY = Mathf.FloorToInt(min.y);
        int width = Mathf.CeilToInt(max.x - min.x);
        int height = Mathf.CeilToInt(max.y - min.y);
        float[,] heights = _terrain.terrainData.GetHeights(baseX, baseY, width, height);

        float placeRate = 0.01f;

        while (_reserves - placeRate > 0)
        {
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    if (_reserves - placeRate > 0)
                    {
                        heights[y, x] += placeRate;
                        _reserves -= placeRate;
                    }
        }

        _terrain.terrainData.SetHeights(baseX, baseY, heights);
        _lastPosition = Vector2.zero;

        // TODO: Cover edge cases, literally
    }

    void LiftFinger(PlayerActions playerActionsScript)
    {
        _lastPosition = Vector2.zero;
    }

    private float[,] RaiseHeightmap(float[,] heights, int numberOfColumns, int numberOfRows, float[,] mask, float reserves)
    {
        float sumOfMask = 0;
        for (int row = 0; row < numberOfRows; row++)
            for (int column = 0; column < numberOfColumns; column++)
                sumOfMask += mask[column, row];

        float reservePerHeightCell = reserves / sumOfMask;

        for (int row = 0; row < numberOfRows; row++)
            for (int column = 0; column < numberOfColumns; column++)
                heights[column, row] = Mathf.Max(0, heights[column, row] + reservePerHeightCell * mask[column, row]);
        return heights;
    }

    private float[,] DepressHeightmap(float digRate, float[,] heights, int numberOfColumns, int numberOfRows, float[,] mask)
    {
        for (int row = 0; row < numberOfRows; row++)
            for (int column = 0; column < numberOfColumns; column++)
                heights[column, row] = Mathf.Max(0, heights[column, row] - digRate * mask[column, row]);
        return heights;
    }

    private float GetReserves(float digRate, float[,] mask)
    {
        float reserves = 0;
        int numberOfColumns = mask.GetLength(0);
        int numberOfRows = mask.GetLength(1);
        for (int row = 0; row < numberOfRows; row++)
            for (int column = 0; column < numberOfColumns; column++)
                reserves += digRate * mask[column, row];
        return reserves;
    }

    private float[,] GetDepressedMask(int digSize, float radius, int numberOfColumns, int numberOfRows, Vector2 center1, Vector2 center2)
    {
        float[,] mask = new float[numberOfColumns, numberOfRows];
        if (digSize > 1)
        {
            //for (int row = 0; row < numberOfRows; row++)
            //    for (int column = 0; column < numberOfColumns; column++)
            //        if (Vector2.Distance(center2, new Vector2(row, column)) <= radius)
            //            mask[column, row] = 0;

            //for (int row = 0; row < numberOfRows; row++)
            //    for (int column = 0; column < numberOfColumns; column++)
            //        if (GetLineDistance(center1, center2, new Vector2(row, column)) <= radius)
            //            if (CheckIfWithinBothHypotenuses(radius, center1, center2, new Vector2(row, column)))
            //                mask[column, row] = 1;

            //for (int row = 0; row < numberOfRows; row++)
            //    for (int column = 0; column < numberOfColumns; column++)
            //        if (Vector2.Distance(center1, new Vector2(row, column)) <= radius)
            //            mask[column, row] = 0;
        }
        else if (digSize == 1)
            mask[0, 0] = 1;
        return mask;
    }

    private float[,] GetRaisedMask(int digSize, float radius, int numberOfColumns, int numberOfRows, Vector2 center1, Vector2 center2)
    {
        float[,] mask = new float[numberOfColumns, numberOfRows];
        if (digSize > 1)
        {
            for (int row = 0; row < numberOfRows; row++)
                for (int column = 0; column < numberOfColumns; column++)
                    if (Vector2.Distance(center2, new Vector2(row, column)) <= radius)
                        mask[column, row] = 1f;
        }
        else if (digSize == 1)
            mask[0, 0] = 1;
        return mask;
    }

    bool IsLastPositionEmpty(Vector2 pointOnTerrain)
    {
        if (_lastPosition == Vector2.zero) // TODO: Specify Vector2.NegativeInifinity
        {
            _lastPosition = pointOnTerrain;
            return true;
        }
        return false;
    }

    bool IsNextActionOutsideThreshold(Vector2 pointOnTerrain, float threshold)
    {
        return Vector2.Distance(_lastPosition, pointOnTerrain) <= threshold;
    }

    private static Vector2 GetMaxBoundry(Vector2 pointOnHeightmap, Vector2 lastPointOnHeightmap, float radius)
    {
        Vector2 max;
        max.x = Mathf.Max(lastPointOnHeightmap.x, pointOnHeightmap.x) + radius;
        max.y = Mathf.Max(lastPointOnHeightmap.y, pointOnHeightmap.y) + radius;
        return max;
    }

    private static Vector2 GetMinBoundry(Vector2 pointOnHeightmap, Vector2 lastPointOnHeightmap, float radius)
    {
        Vector2 min;
        min.x = Mathf.Min(lastPointOnHeightmap.x, pointOnHeightmap.x) - radius;
        min.y = Mathf.Min(lastPointOnHeightmap.y, pointOnHeightmap.y) - radius;
        return min;
    }

    Vector2 GetPointOnHeightMap(Vector2 pointOnTerrain)
    {
        Vector2 pointOnHeightmap;
        pointOnHeightmap.x = pointOnTerrain.x / _terrain.terrainData.size.x * _terrain.terrainData.heightmapWidth;
        pointOnHeightmap.y = pointOnTerrain.y / _terrain.terrainData.size.z * _terrain.terrainData.heightmapHeight;
        return pointOnHeightmap;
    }

    Vector2 GetSizeOnHeightMap(Vector2 size)
    {
        Vector2 sizeOnHeightMap;
        sizeOnHeightMap.x = _terrain.terrainData.heightmapWidth / _terrain.terrainData.size.x * size.x;
        sizeOnHeightMap.y = _terrain.terrainData.heightmapHeight / _terrain.terrainData.size.z * size.y;
        return sizeOnHeightMap;
    }
}

