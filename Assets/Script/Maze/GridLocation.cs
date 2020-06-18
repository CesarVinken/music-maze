using UnityEngine;

public struct GridLocation
{
    public float X;
    public float Y;

    public GridLocation(float x, float y)
    {
        X = x;
        Y = y;
    }

    public static Vector2 GridToVector(GridLocation gridLocation)
    {
        float xAxisMultiplier = 1f;
        float yAxisMultiplier = 1f;

        return new Vector2(gridLocation.X * xAxisMultiplier, gridLocation.Y * yAxisMultiplier);
    }
}