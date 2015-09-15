using UnityEngine;
using System.Collections;
using System;

public class BrushTool
{
    public static float[,] GetSquareMask(int width, int height, float value = 1)
    {
        float[,] squareMask = new float[width, height];
        for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
                squareMask[i, j] = value;

        return squareMask;
    }

    public static float[,] GetCircleMask(int diameter, float value = 1)
    {
        float[,] circleMask = new float[diameter, diameter];
        float radius = diameter / 2;
        Vector2 center = new Vector2(radius, radius);
        for (int i = 0; i < diameter; i++)
            for (int j = 0; j < diameter; j++)
                if (Vector2.Distance(new Vector2(i + 0.5f, j + 0.5f), center) <= radius)
                    circleMask[i, j] = value;

        return circleMask;
    }

    public static float[,] GetCircularGradientOutlineMask(int diameter, int innerDiameter, float value = 1)
    {
        // TODO: Assert innerDiameter < diameter

        float[,] circleGradientOutlineMask = new float[diameter, diameter];
        float radius = diameter / 2;
        float innerRadius = innerDiameter / 2;
        float deltaRadius = radius - innerRadius;
        Vector2 center = new Vector2(radius, radius);
        for (int i = 0; i < diameter; i++)
            for (int j = 0; j < diameter; j++)
            {
                Vector2 cell = new Vector2(i + 0.5f, j + 0.5f);
                float distance = Vector2.Distance(cell, center);
                if (distance < radius && distance > innerRadius)
                {
                    float middleOfOutline = innerRadius + deltaRadius / 2;
                    float distanceRatio = 1 - (Mathf.Abs(distance - middleOfOutline) / (deltaRadius / 2));
                    distanceRatio = Mathf.Clamp01(distanceRatio);
                    circleGradientOutlineMask[i, j] = value * distanceRatio;
                }
            }

        return circleGradientOutlineMask;
    }

    public static float[,] DrawLine(Vector2 point1, Vector2 point2, float size, float value = 1)
    {
        float radius = size / 2;
        float width = point2.x - point1.x + size;
        float height = point2.y - point1.y + size;
        int widthInt = Mathf.CeilToInt(width);
        int heightInt = Mathf.CeilToInt(height);
        float[,] lineMask = new float[widthInt, heightInt];

        Vector2 localPoint1 = GetLocalPoint(point1, point2, radius, width, height);
        Vector2 localPoint2 = GetLocalPoint(point2, point1, radius, width, height);

        for (int x = 0; x < widthInt; x++)
            for (int y = 0; y < heightInt; y++)
                if (Vector2.Distance(localPoint1, new Vector2(x, y)) <= radius) // This makes it a circle!
                    lineMask[x, y] = value;
                else if (Vector2.Distance(localPoint2, new Vector2(x, y)) <= radius) // This makes it a circle!
                    lineMask[x, y] = value;
                else if (GetLineDistance(localPoint1, localPoint2, new Vector2(x, y)) <= radius)
                    if (CheckIfWithinBothHypotenuses(radius, localPoint1, localPoint2, new Vector2(x, y)))
                        lineMask[x, y] = value;

        return lineMask;
    }

    static Vector2 GetLocalPoint(Vector2 point, Vector2 otherPoint, float radius, float width, float height)
    {
        Vector2 localPoint;
        localPoint.x = point.x < otherPoint.x ? radius : width - radius;
        localPoint.y = point.y < otherPoint.y ? radius : height - radius;
        return localPoint;
    }

    public static float ApplyMask(float[,] heights, float[,] mask, float rate, bool flipXY = true)
    {
        int heightsWidth= flipXY ? heights.GetLength(1) : heights.GetLength(0);
        int heightsHeight = flipXY ? heights.GetLength(0) : heights.GetLength(1);
        int maskWidth = mask.GetLength(0);
        int maskHeight = mask.GetLength(1);
        int offsetX = Mathf.FloorToInt((heightsWidth - maskWidth) / 2f);
        int offsetY = Mathf.FloorToInt((heightsHeight - maskHeight) / 2f);
        if (maskWidth > heightsWidth || maskHeight > heightsHeight)
            throw new ArgumentException("Mask must be smaller or equal to Heights");

        float amountModified = 0;
        for (int x = 0; x < mask.GetLength(0); x++)
            for (int y = 0; y < mask.GetLength(1); y++)
            {
                amountModified += mask[x, y] * rate;
                if (flipXY)
                    heights[y + offsetY, x + offsetX] += mask[x, y] * rate;
                else
                    heights[x + offsetX, y + offsetY] += mask[x, y] * rate;
            }
        return amountModified;
    }

    public static float ApplyRate(float[,] heights, float rate)
    {
        float amountModified = 0;
        for (int y = 0; y < heights.GetLength(0); y++)
            for (int x = 0; x < heights.GetLength(1); x++)
            {
                heights[y, x] += rate;
                amountModified += rate;
            }
        return amountModified;
    }

    public static float GetLineDistance(Vector2 center1, Vector2 center2, Vector2 point)
    {
        Vector2 delta;
        delta.x = center2.x - center1.x;
        delta.y = center2.y - center1.y;
        float numerator = Mathf.Abs(delta.y * point.x - delta.x * point.y + center2.x * center1.y - center1.x * center2.y);
        float denomentator = Mathf.Sqrt(Mathf.Pow(delta.y, 2) + Mathf.Pow(delta.x, 2));
        return numerator / denomentator;
    }

    public static bool CheckIfWithinBothHypotenuses(float radius, Vector2 center1, Vector2 center2, Vector2 point)
    {
        float aSquared = radius * radius;
        float bSquared = Vector2.SqrMagnitude(center2 - center1);
        float hypotenuseSquared = aSquared + bSquared;
        float hypotenuse = Mathf.Sqrt(hypotenuseSquared);
        bool check1 = Vector2.Distance(center1, point) <= hypotenuse;
        bool check2 = Vector2.Distance(center2, point) <= hypotenuse;
        return check1 && check2;
    }

    public static float GetTotalValue(float[,] array)
    {
        float sumOfArray = 0;
        for (int x = 0; x < array.GetLength(0); x++)
            for (int y = 0; y < array.GetLength(0); y++)
                sumOfArray += array[x, y];
        return sumOfArray;
    }
}
