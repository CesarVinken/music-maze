using System.Collections.Generic;

public class NeighbourTileCalculator
{
    public static int MapNeighbourObstaclesOfTile(Tile tile, ObstacleType obstacleType, bool isDoor)
    {
        Logger.Log($"Map neighbours of {tile.GridLocation.X},{tile.GridLocation.Y}");
        bool obstacleRight = false;
        bool obstacleDown = false;
        bool obstacleLeft = false;
        bool obstacleUp = false;
        if (!EditorManager.InEditor)
        {
            Logger.Error("MapNeighbourObstaclesOnSelf was not called from the editor");
            return -1;
        }

        foreach (KeyValuePair<ObjectDirection, Tile> neighbour in tile.Neighbours)
        {
            Logger.Warning($"Neighbour at {neighbour.Value.GridLocation.X}, {neighbour.Value.GridLocation.Y} is {neighbour.Key} of {tile.GridLocation.X},{tile.GridLocation.Y}");
            if (neighbour.Key == ObjectDirection.Right)
            {
                TileObstacle tileObstacle = neighbour.Value.TryGetTileObstacle();
                if (tileObstacle != null && tileObstacle.ObstacleType == obstacleType)
                {
                    obstacleRight = true;
                }
            }
            else if (neighbour.Key == ObjectDirection.Down)
            {
                TileObstacle tileObstacle = neighbour.Value.TryGetTileObstacle();
                if (tileObstacle != null && tileObstacle.ObstacleType == obstacleType)
                {
                    obstacleDown = true;
                }
            }
            else if (neighbour.Key == ObjectDirection.Left)
            {
                TileObstacle tileObstacle = neighbour.Value.TryGetTileObstacle();
                if (tileObstacle != null && tileObstacle.ObstacleType == obstacleType)
                {
                    obstacleLeft = true;
                }
            }
            else if (neighbour.Key == ObjectDirection.Up)
            {
                TileObstacle tileObstacle = neighbour.Value.TryGetTileObstacle();
                if (tileObstacle != null && tileObstacle.ObstacleType == obstacleType)
                {
                    obstacleUp = true;
                }
            }
        }

        if (isDoor)
        {
            return CalculateDoorConnectionScore(obstacleRight, obstacleDown, obstacleLeft, obstacleUp);
        }
        return CalculateObstacleConnectionScore(obstacleRight, obstacleDown, obstacleLeft, obstacleUp);
    }

    private static int CalculateDoorConnectionScore(bool right, bool down, bool left, bool up)
    {
        if (right)
        {
            if (left)
            {
                return 4;
            }
            return 2;
        }
        if (left)
        {
            return 3;
        }
        if (down)
        {
            if (up)
            {
                return 8;
            }
            return 6;
        }
        if (up)
        {
            return 7;
        }
        return 1;
    }

    private static int CalculateObstacleConnectionScore(bool right, bool down, bool left, bool up)
    {
        if (right)
        {
            if (down)
            {
                if (left)
                {
                    if (up)
                    {
                        return 16;
                    }
                    return 12;
                }
                else if (up)
                {
                    return 13;
                }
                return 6;
            }
            else if (left)
            {
                if (up)
                {
                    return 14;
                }
                return 7;
            }
            else if (up)
            {
                return 8;
            }
            return 2;
        }
        else if (down)
        {
            if (left)
            {
                if (up)
                {
                    return 15;
                }
                return 10;
            }
            else if (up)
            {
                return 9;
            }
            return 3;
        }
        else if (left)
        {
            if (up)
            {
                return 11;
            }
            return 4;
        }
        else if (up)
        {
            return 5;
        }

        return 1;
    }
}