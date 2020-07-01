using UnityEngine;

public struct GridLocation
{
    public int X;
    public int Y;

    private static float _xAxisMultiplier = 1f;
    private static float _yAxisMultiplier = 1f;

    public static float OffsetToTileMiddle = 0.5f;


    public GridLocation(int gridX, int gridY) // x and y are not necessarily equal to grid coordinates!
    {
        X = gridX;
        Y = gridY;
    }

    public static Vector2 GridToVector(GridLocation gridLocation)
    {
        Logger.Log("Target grid location: {0}, {1}", gridLocation.X, gridLocation.Y);

        return new Vector2(gridLocation.X * _xAxisMultiplier, gridLocation.Y * _yAxisMultiplier);
    }

    public static GridLocation VectorToGrid(Vector2 vectorLocation)
    {
        return new GridLocation((int)(vectorLocation.x / 1f), (int)(vectorLocation.y / 1f));
    }

    public static GridLocation FindClosestGridTile(Vector2 vectorLocation)
    {
        float flooredX = Mathf.Floor(vectorLocation.x);
        float flooredY = Mathf.Floor(vectorLocation.y);

        float gridX = flooredX;
        float gridY = flooredY;
        if (flooredX > 0)
        {
            gridX = flooredX - (flooredX % _xAxisMultiplier);
        }
        else if (flooredX < 0)
        {
            gridX = flooredX - (((flooredX % _xAxisMultiplier) * -1));
        }

        if (flooredY > 0)
        {
            gridY = flooredY - (flooredY % _yAxisMultiplier);
        }
        else if (flooredY < 0)
        {
            gridY = flooredY - (((flooredY % _yAxisMultiplier) * -1));
        }

        return new GridLocation((int)gridX, (int)gridY);
    }
}