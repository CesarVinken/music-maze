using System.Collections.Generic;

public class NeighbourTileCalculator
{
    public static int MapNeighbourPathsOfTile(Tile tile, MazeTilePathType pathType)
    {
        Logger.Log($"Map neighbours of {tile.GridLocation.X},{tile.GridLocation.Y}");
        TileConnectionWidth pathRight = TileConnectionWidth.None;
        TileConnectionWidth pathDown = TileConnectionWidth.None;
        TileConnectionWidth pathLeft = TileConnectionWidth.None;
        TileConnectionWidth pathUp = TileConnectionWidth.None;

        if (!EditorManager.InEditor)
        {
            Logger.Error("MapNeighbourPathsOfTile was not called from the editor");
            return -1;
        }

        foreach (KeyValuePair<ObjectDirection, Tile> neighbour in tile.Neighbours)
        {
            Logger.Warning($"Neighbour at {neighbour.Value.GridLocation.X}, {neighbour.Value.GridLocation.Y} is {neighbour.Key} of {tile.GridLocation.X},{tile.GridLocation.Y}");
            if (neighbour.Key == ObjectDirection.Right)
            {
                MazeTilePath tilePath = neighbour.Value.TryGetTilePath();
                if (tilePath != null && tilePath.MazeTilePathType == pathType)
                {
                    // Check if the neighbour has a Full connection width on its left side (so the right side connection for THIS tile)
                    if (tilePath._pathConnectionScore == 16 ||
                        tilePath._pathConnectionScore == 19 ||
                        tilePath._pathConnectionScore == 22 ||
                        tilePath._pathConnectionScore == 25 ||
                        tilePath._pathConnectionScore == 26 ||
                        tilePath._pathConnectionScore == 29 ||
                        tilePath._pathConnectionScore == 31 ||
                        tilePath._pathConnectionScore == 33 ||
                        tilePath._pathConnectionScore == 34)
                    {
                        pathRight = TileConnectionWidth.Full;
                    }
                    else
                    {
                        pathRight = TileConnectionWidth.Normal;
                    }
                }
            }
            else if (neighbour.Key == ObjectDirection.Down)
            {
                MazeTilePath tilePath = neighbour.Value.TryGetTilePath();
                if (tilePath != null && tilePath.MazeTilePathType == pathType)
                {
                    if (tilePath._pathConnectionScore == 16 ||
                        tilePath._pathConnectionScore == 20 ||
                        tilePath._pathConnectionScore == 23 ||
                        tilePath._pathConnectionScore == 24 ||
                        tilePath._pathConnectionScore == 26 ||
                        tilePath._pathConnectionScore == 30 ||
                        tilePath._pathConnectionScore == 32 ||
                        tilePath._pathConnectionScore == 33 ||
                        tilePath._pathConnectionScore == 34)
                    {
                        pathDown = TileConnectionWidth.Full;
                    }
                    else
                    {
                        pathDown = TileConnectionWidth.Normal;
                    }
                }
            }
            else if (neighbour.Key == ObjectDirection.Left)
            {
                MazeTilePath tilePath = neighbour.Value.TryGetTilePath();
                if (tilePath != null && tilePath.MazeTilePathType == pathType)
                {
                    if (tilePath._pathConnectionScore == 16 ||
                        tilePath._pathConnectionScore == 17 ||
                        tilePath._pathConnectionScore == 21 ||
                        tilePath._pathConnectionScore == 22 ||
                        tilePath._pathConnectionScore == 23 ||
                        tilePath._pathConnectionScore == 27 ||
                        tilePath._pathConnectionScore == 31 ||
                        tilePath._pathConnectionScore == 32 ||
                        tilePath._pathConnectionScore == 33)
                    {
                        pathLeft = TileConnectionWidth.Full;
                    }
                    else
                    {
                        pathLeft = TileConnectionWidth.Normal;
                    }
                }
            }
            else if (neighbour.Key == ObjectDirection.Up)
            {
                MazeTilePath tilePath = neighbour.Value.TryGetTilePath();
                if (tilePath != null && tilePath.MazeTilePathType == pathType)
                {
                    if (tilePath._pathConnectionScore == 16 ||
                        tilePath._pathConnectionScore == 18 ||
                        tilePath._pathConnectionScore == 21 ||
                        tilePath._pathConnectionScore == 24 ||
                        tilePath._pathConnectionScore == 25 ||
                        tilePath._pathConnectionScore == 28 ||
                        tilePath._pathConnectionScore == 31 ||
                        tilePath._pathConnectionScore == 32 ||
                        tilePath._pathConnectionScore == 34)
                    {
                        pathUp = TileConnectionWidth.Full;
                    }
                    else
                    {
                        pathUp = TileConnectionWidth.Normal;
                    }
                }
            }
        }

        return CalculatePathConnectionScore(pathRight, pathDown, pathLeft, pathUp);
    }

    public static int MapNeighbourObstaclesOfTile(Tile tile, ObstacleType obstacleType, bool isDoor)
    {
        Logger.Log($"Map neighbours of {tile.GridLocation.X},{tile.GridLocation.Y}");
        bool obstacleRight = false;
        bool obstacleDown = false;
        bool obstacleLeft = false;
        bool obstacleUp = false;
        if (!EditorManager.InEditor)
        {
            Logger.Error("MapNeighbourObstaclesOfTile was not called from the editor");
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

    private static int CalculatePathConnectionScore(TileConnectionWidth right, TileConnectionWidth down, TileConnectionWidth left, TileConnectionWidth up)
    {
        if (right == TileConnectionWidth.Normal) 
        {
            if(left == TileConnectionWidth.Full)
            {
                return 29;
            }
            if (down == TileConnectionWidth.Normal)
            {
                if (left == TileConnectionWidth.Normal)
                {
                    if (up == TileConnectionWidth.Normal)
                    {
                        return 16;
                    }
                    return 12;
                }
                if (up == TileConnectionWidth.Normal)
                {
                    return 13;
                }
                return 6;
            }
            if (left == TileConnectionWidth.Normal)
            {
                if (up == TileConnectionWidth.Normal)
                {
                    return 14;
                }
                return 7;
            }
            if (up == TileConnectionWidth.Normal)
            {
                return 8;
            }
            return 2;
        }
        if (down == TileConnectionWidth.Normal)
        {
            if(up == TileConnectionWidth.Full)
            {
                return 30;
            }
            if (up == TileConnectionWidth.Normal)
            {
                return 9;
            }
            if (left == TileConnectionWidth.Normal)
            {
                if (up == TileConnectionWidth.Normal)
                {
                    return 15;
                }
                return 10;
            }
            return 3;
        }
        if (left == TileConnectionWidth.Normal)
        {
            if (up == TileConnectionWidth.Normal)
            {
                return 11;
            }
            return 4;
        }
        if (up == TileConnectionWidth.Normal)
        {
            return 5;
        }
        if (right == TileConnectionWidth.Full)
        {
            if (left == TileConnectionWidth.Normal)
            {
                return 27;
            }
            if(down == TileConnectionWidth.Full)
            {
                if(left == TileConnectionWidth.Full)
                {
                    return 31;
                }
                if(up == TileConnectionWidth.Full)
                {
                    return 32;
                }
                return 21;
            }
            if (left == TileConnectionWidth.Full)
            {
                if(up == TileConnectionWidth.Full)
                {
                    return 33;
                }
                return 22;
            }
            if (up == TileConnectionWidth.Full)
            {
                return 23;
            }
            return 17;
        }
        if (down == TileConnectionWidth.Full)
        {
            if(up == TileConnectionWidth.Normal)
            {
                return 28;
            }
            if (up == TileConnectionWidth.Full)
            {
                return 24;
            }
            if (left == TileConnectionWidth.Full)
            {
                if(up == TileConnectionWidth.Full)
                {
                    return 34;
                }
                return 25;
            }
            return 18;
        }
        if (left == TileConnectionWidth.Full)
        {
            if (up == TileConnectionWidth.Full)
            {
                return 26;
            }
            return 19;
        }
        if (up == TileConnectionWidth.Full)
        {
            return 20;
        }
        return 1;
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