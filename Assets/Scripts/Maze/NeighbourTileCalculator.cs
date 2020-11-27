using System.Collections.Generic;

public struct TileConnectionInfo
{
    public Direction Direction;
    public bool HasConnection;
    public int NeighbourConnectionScore; // the connection score of the opposing tile.

    public TileConnectionInfo(Direction direction, bool hasConnection = false, int neighbourConnectionScore = -1)
    {
        Direction = direction;
        HasConnection = hasConnection;
        NeighbourConnectionScore = neighbourConnectionScore;
    }
}

public class NeighbourTileCalculator
{
    public static int MapNeighbourPathsOfTile(Tile tile, MazeTilePathType pathType)
    {
        Logger.Log($"---------Map neighbours of {tile.GridLocation.X},{tile.GridLocation.Y}--------");
        TileConnectionInfo pathRight = new TileConnectionInfo(Direction.Right);
        TileConnectionInfo pathDown = new TileConnectionInfo(Direction.Down);
        TileConnectionInfo pathLeft = new TileConnectionInfo(Direction.Left);
        TileConnectionInfo pathUp = new TileConnectionInfo(Direction.Up);

        if (!EditorManager.InEditor)
        {
            Logger.Error("MapNeighbourPathsOfTile was not called from the editor");
            return -1;
        }

        foreach (KeyValuePair<ObjectDirection, Tile> neighbour in tile.Neighbours)
        {
            Logger.Warning($"Neighbour at {neighbour.Value.GridLocation.X},{neighbour.Value.GridLocation.Y} is {neighbour.Key} of {tile.GridLocation.X},{tile.GridLocation.Y}");
            if (neighbour.Key == ObjectDirection.Right)
            {
                MazeTilePath tilePath = neighbour.Value.TryGetTilePath();
                if (tilePath != null && tilePath.MazeTilePathType == pathType)
                {
                    pathRight.HasConnection = true;
                    pathRight.NeighbourConnectionScore = tilePath.PathConnectionScore;
                }
            }
            else if (neighbour.Key == ObjectDirection.Down)
            {
                MazeTilePath tilePath = neighbour.Value.TryGetTilePath();
                if (tilePath != null && tilePath.MazeTilePathType == pathType)
                {
                    pathDown.HasConnection = true;
                    pathDown.NeighbourConnectionScore = tilePath.PathConnectionScore;                    
                }
            }
            else if (neighbour.Key == ObjectDirection.Left)
            {
                MazeTilePath tilePath = neighbour.Value.TryGetTilePath();
                if (tilePath != null && tilePath.MazeTilePathType == pathType)
                {
                    pathLeft.HasConnection = true;
                    pathLeft.NeighbourConnectionScore = tilePath.PathConnectionScore;
                }
            }
            else if (neighbour.Key == ObjectDirection.Up)
            {
                MazeTilePath tilePath = neighbour.Value.TryGetTilePath();
                if (tilePath != null && tilePath.MazeTilePathType == pathType)
                {
                    pathUp.HasConnection = true;
                    pathUp.NeighbourConnectionScore = tilePath.PathConnectionScore;
                }
            }
        }

        return CalculatePathConnectionScore(pathRight, pathDown, pathLeft, pathUp);
    }

    public static int MapNeighbourObstaclesOfTile(Tile tile, ObstacleType obstacleType, bool isDoor)
    {
        Logger.Log($"Map neighbours of {tile.GridLocation.X},{tile.GridLocation.Y}");
        bool obstacleRight = false; // TODO turn bools in to TileConnectionWidth
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

    private static int CalculatePathConnectionScore(TileConnectionInfo right, TileConnectionInfo down, TileConnectionInfo left, TileConnectionInfo up)
    {
        if (right.HasConnection)
        {
            if (down.HasConnection)
            {
                if (left.HasConnection)
                {
                    if (up.HasConnection)
                    {
                        return 16;
                    }
                    return 31;
                }
                if (up.HasConnection)
                {
                    return 32;
                }
                return 21;
            }
            if (left.HasConnection)
            {
                if (up.HasConnection)
                {
                    return 33;
                }
                return 22;
            }
            if (up.HasConnection)
            {
                return 23;
            }
            return 17;
        }
        if (down.HasConnection)
        {
            if(left.HasConnection)
            {
                if (up.HasConnection)
                {
                    return 34;
                }
                return 25;
            }
            if (up.HasConnection)
            {
                return 24;
            }
            return 18;
        }
        if (left.HasConnection)
        {
            if (up.HasConnection)
            {
                return 26;
            }
            return 19;
        }
        if (up.HasConnection)
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