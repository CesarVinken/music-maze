using System.Collections.Generic;

public struct TilePathConnectionInfo
{
    public Direction Direction;
    public bool HasConnection;
    public MazeTilePath MazeTilePath;

    public TilePathConnectionInfo(Direction direction, bool hasConnection = false, MazeTilePath mazeTilePath = null)
    {
        Direction = direction;
        HasConnection = hasConnection;
        MazeTilePath = mazeTilePath;
    }
}

public class NeighbourTileCalculator
{
    public static readonly int ConnectionOnAllSidesScore = 16;

    public static int MapNeighbourPathsOfTile(Tile tile, MazeTilePathType pathType)
    {
        Logger.Log($"---------Map neighbours of {tile.GridLocation.X},{tile.GridLocation.Y}--------");
        TilePathConnectionInfo pathRight = new TilePathConnectionInfo(Direction.Right);
        TilePathConnectionInfo pathDown = new TilePathConnectionInfo(Direction.Down);
        TilePathConnectionInfo pathLeft = new TilePathConnectionInfo(Direction.Left);
        TilePathConnectionInfo pathUp = new TilePathConnectionInfo(Direction.Up);

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
                    pathRight.MazeTilePath = tilePath;
                }
            }
            else if (neighbour.Key == ObjectDirection.Down)
            {
                MazeTilePath tilePath = neighbour.Value.TryGetTilePath();
                if (tilePath != null && tilePath.MazeTilePathType == pathType)
                {
                    pathDown.HasConnection = true;
                    pathDown.MazeTilePath = tilePath;
                }
            }
            else if (neighbour.Key == ObjectDirection.Left)
            {
                MazeTilePath tilePath = neighbour.Value.TryGetTilePath();
                if (tilePath != null && tilePath.MazeTilePathType == pathType)
                {
                    pathLeft.HasConnection = true;
                    pathLeft.MazeTilePath = tilePath;
                }
            }
            else if (neighbour.Key == ObjectDirection.Up)
            {
                MazeTilePath tilePath = neighbour.Value.TryGetTilePath();
                if (tilePath != null && tilePath.MazeTilePathType == pathType)
                {
                    pathUp.HasConnection = true;
                    pathUp.MazeTilePath = tilePath;
                }
            }
        }

        return CalculatePathConnectionScore(pathRight, pathDown, pathLeft, pathUp);
    }

    public static List<MazeTilePath> GetUpdatedTilePathsForVariation(Tile tile, MazeTilePath thisMazeTilePath, MazeTilePathType pathType)
    {
        Logger.Log($"---------Map neighbours of {tile.GridLocation.X},{tile.GridLocation.Y}--------");
        TilePathConnectionInfo pathRight = new TilePathConnectionInfo(Direction.Right);
        TilePathConnectionInfo pathDown = new TilePathConnectionInfo(Direction.Down);
        TilePathConnectionInfo pathLeft = new TilePathConnectionInfo(Direction.Left);
        TilePathConnectionInfo pathUp = new TilePathConnectionInfo(Direction.Up);

        foreach (KeyValuePair<ObjectDirection, Tile> neighbour in tile.Neighbours)
        {
            Logger.Warning($"Neighbour at {neighbour.Value.GridLocation.X},{neighbour.Value.GridLocation.Y} is {neighbour.Key} of {tile.GridLocation.X},{tile.GridLocation.Y}");
            if (neighbour.Key == ObjectDirection.Right)
            {
                MazeTilePath tilePath = neighbour.Value.TryGetTilePath();
                if (tilePath != null && tilePath.MazeTilePathType == pathType)
                {
                    pathRight.HasConnection = true;
                    pathRight.MazeTilePath = tilePath;
                }
            }
            else if (neighbour.Key == ObjectDirection.Down)
            {
                MazeTilePath tilePath = neighbour.Value.TryGetTilePath();
                if (tilePath != null && tilePath.MazeTilePathType == pathType)
                {
                    pathDown.HasConnection = true;
                    pathDown.MazeTilePath = tilePath;
                }
            }
            else if (neighbour.Key == ObjectDirection.Left)
            {
                MazeTilePath tilePath = neighbour.Value.TryGetTilePath();
                if (tilePath != null && tilePath.MazeTilePathType == pathType)
                {
                    pathLeft.HasConnection = true;
                    pathLeft.MazeTilePath = tilePath;
                }
            }
            else if (neighbour.Key == ObjectDirection.Up)
            {
                MazeTilePath tilePath = neighbour.Value.TryGetTilePath();
                if (tilePath != null && tilePath.MazeTilePathType == pathType)
                {
                    pathUp.HasConnection = true;
                    pathUp.MazeTilePath = tilePath;
                }
            }
        }

        return CalculatePathConnectionScoreForVariations(thisMazeTilePath, pathRight, pathDown, pathLeft, pathUp);
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

    private static int CalculatePathConnectionScore(TilePathConnectionInfo right, TilePathConnectionInfo down, TilePathConnectionInfo left, TilePathConnectionInfo up)
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

    private static List<MazeTilePath> CalculatePathConnectionScoreForVariations(MazeTilePath thisTilePath, TilePathConnectionInfo right, TilePathConnectionInfo down, TilePathConnectionInfo left, TilePathConnectionInfo up)
    {
        List<MazeTilePath> updatedConnections = new List<MazeTilePath>();

        int currentScoreThisTilePath = thisTilePath.PathConnectionScore;
        int currentScoreRight = right.MazeTilePath ? right.MazeTilePath.PathConnectionScore : -1;
        int currentScoreDown = down.MazeTilePath ? down.MazeTilePath.PathConnectionScore : -1;
        int currentScoreLeft = left.MazeTilePath? left.MazeTilePath.PathConnectionScore : -1;
        int currentScoreUp = up.MazeTilePath ? up.MazeTilePath.PathConnectionScore : -1;

        if (currentScoreThisTilePath == 2)
        {
            if(currentScoreRight == 22)
            {
                right.MazeTilePath.PathConnectionScore = 27;
            }
            else if(currentScoreRight == 29)
            {
                right.MazeTilePath.PathConnectionScore = 7;
            }
        }

        if (currentScoreThisTilePath == 3)
        {
            if(currentScoreDown == 24)
            {
                down.MazeTilePath.PathConnectionScore = 28;
            }
            else if(currentScoreDown == 30)
            {
                down.MazeTilePath.PathConnectionScore = 10;
            }
        }

        if (currentScoreThisTilePath == 4)
        {
            if (currentScoreLeft == 22)
            {
                left.MazeTilePath.PathConnectionScore = 29;
            }
            else if(currentScoreLeft == 27)
            {
                left.MazeTilePath.PathConnectionScore = 7;
            }
        }

        if (currentScoreThisTilePath == 5)
        {
            if(currentScoreUp == 24)
            {
                up.MazeTilePath.PathConnectionScore = 30;
            }
            else if(currentScoreUp == 28)
            {
                up.MazeTilePath.PathConnectionScore = 10;
            }
        }

        if (currentScoreThisTilePath == 7)
        {
            if(currentScoreRight == 19)
            {
                right.MazeTilePath.PathConnectionScore = 4;
            }
            else if (currentScoreRight == 22)
            {
                right.MazeTilePath.PathConnectionScore = 27;
            }
            else if(currentScoreRight == 29)
            {
                right.MazeTilePath.PathConnectionScore = 7;
            }

            if(currentScoreLeft == 17)
            {
                left.MazeTilePath.PathConnectionScore = 2;
            }
            else if (currentScoreLeft == 22)
            {
                left.MazeTilePath.PathConnectionScore = 29;
            }
            else if(currentScoreLeft == 27)
            {
                left.MazeTilePath.PathConnectionScore = 7;
            }
        }

        if (currentScoreThisTilePath == 10)
        {
            if(currentScoreDown == 20)
            {
                down.MazeTilePath.PathConnectionScore = 5;
            }
            else if (currentScoreDown == 24)
            {
                down.MazeTilePath.PathConnectionScore = 28;
            }
            else if(currentScoreDown == 30)
            {
                down.MazeTilePath.PathConnectionScore = 10;
            }

            if(currentScoreUp == 18)
            {
                up.MazeTilePath.PathConnectionScore = 3;
            }
            else if (currentScoreUp == 24)
            {
                up.MazeTilePath.PathConnectionScore = 30;
            }
            else if(currentScoreUp == 28)
            {
                up.MazeTilePath.PathConnectionScore = 10;
            }
        }

        if (currentScoreThisTilePath == 16)
        {
            if(currentScoreRight == 7)
            {
                right.MazeTilePath.PathConnectionScore = 29;
            }
            else if (currentScoreLeft == 27)
            {
                right.MazeTilePath.PathConnectionScore = 22;
            }

            if(currentScoreDown == 10)
            {
                down.MazeTilePath.PathConnectionScore = 30;
            }
            else if(currentScoreDown == 28)
            {
                down.MazeTilePath.PathConnectionScore = 24;
            }

            if(currentScoreLeft == 7)
            {
                left.MazeTilePath.PathConnectionScore = 27;
            }
            else if(currentScoreLeft == 27)
            {
                left.MazeTilePath.PathConnectionScore = 22;
            }

            if(currentScoreUp == 10)
            {
                up.MazeTilePath.PathConnectionScore = 28;
            }
            else if(currentScoreUp == 30)
            {
                up.MazeTilePath.PathConnectionScore = 24;
            }
        }

        if (currentScoreThisTilePath == 17)
        {
            if (currentScoreRight == 16 || currentScoreRight == 25 || currentScoreRight == 26 || currentScoreRight == 31 || currentScoreRight == 32 || currentScoreRight == 33 || currentScoreRight == 34)
            {

            }
            else
            {
                thisTilePath.PathConnectionScore = 2;

                if(currentScoreRight == 19)
                {
                    right.MazeTilePath.PathConnectionScore = 4;
                }
                else if (currentScoreRight == 22)
                {
                    right.MazeTilePath.PathConnectionScore = 27;
                }
                else if (currentScoreRight == 29)
                {
                    right.MazeTilePath.PathConnectionScore = 27;
                }
            }
        }

        if (currentScoreThisTilePath == 18)
        {
            if (currentScoreDown == 16 || currentScoreDown == 23 || currentScoreDown == 25 || currentScoreDown == 26 || currentScoreDown == 32 || currentScoreDown == 33 || currentScoreDown == 34)
            {

            }
            else
            {
                thisTilePath.PathConnectionScore = 3;

                if (currentScoreDown == 20)
                {
                    down.MazeTilePath.PathConnectionScore = 5;
                }
                else if (currentScoreDown == 24)
                {
                    down.MazeTilePath.PathConnectionScore = 28;
                }
                else if (currentScoreDown == 30)
                {
                    down.MazeTilePath.PathConnectionScore = 10;
                }
            }
        }

        if (currentScoreThisTilePath == 19)
        {
            if(currentScoreLeft == 16 || currentScoreLeft == 21 || currentScoreLeft == 23 || currentScoreLeft == 31 || currentScoreLeft == 32 || currentScoreLeft == 33)
            {

            }
            else
            {
                thisTilePath.PathConnectionScore = 4;

                if (currentScoreLeft == 17)
                {
                    left.MazeTilePath.PathConnectionScore = 2;
                }
                else if (currentScoreLeft == 22)
                {
                    left.MazeTilePath.PathConnectionScore = 29;
                }
                else if (currentScoreLeft == 27)
                {
                    left.MazeTilePath.PathConnectionScore = 7;
                }
            }
        }

        else if (currentScoreThisTilePath == 20)
        {
            if(currentScoreUp == 16 || currentScoreUp == 21 || currentScoreUp == 25 || currentScoreUp == 31 || currentScoreUp == 32 || currentScoreUp == 34)
            {

            }
            else
            {
                thisTilePath.PathConnectionScore = 5;

                if (currentScoreUp == 18)
                {
                    up.MazeTilePath.PathConnectionScore = 5;
                }
                else if (currentScoreUp == 24)
                {
                    up.MazeTilePath.PathConnectionScore = 30;
                }
                else if(currentScoreUp == 28)
                {
                    up.MazeTilePath.PathConnectionScore = 10;
                }
            }
        }

        else if (currentScoreThisTilePath == 21)
        {
            if (currentScoreRight == 16 || currentScoreRight == 25 || currentScoreRight == 26 || currentScoreRight == 31 || currentScoreRight == 33 || currentScoreRight == 34 ||
                currentScoreDown == 16 || currentScoreDown == 23 || currentScoreDown == 26 || currentScoreDown == 32 || currentScoreDown == 33 || currentScoreDown == 34)
            {

            }
            else
            {
                thisTilePath.PathConnectionScore = 6;

                if(currentScoreRight == 19)
                {
                    right.MazeTilePath.PathConnectionScore = 4;
                }
                else if (currentScoreRight == 22)
                {
                    right.MazeTilePath.PathConnectionScore = 27;
                }
                else if (currentScoreRight == 29)
                {
                    right.MazeTilePath.PathConnectionScore = 7;
                }

                if(currentScoreDown == 20)
                {
                    down.MazeTilePath.PathConnectionScore = 5;
                }
                else if (currentScoreDown == 24)
                {
                    down.MazeTilePath.PathConnectionScore = 28;
                }
                else if (currentScoreDown == 30)
                {
                    down.MazeTilePath.PathConnectionScore = 10;
                }
            }
        }

        else if (currentScoreThisTilePath == 22)
        {
            if ((currentScoreLeft == 16 || currentScoreLeft == 21 || currentScoreLeft == 23 || currentScoreLeft == 31 || currentScoreLeft == 32 || currentScoreLeft == 33) &&
                (currentScoreRight == 16 || currentScoreRight == 25 || currentScoreRight == 26 || currentScoreRight == 31 || currentScoreRight == 33 || currentScoreRight == 34))
            {

            }
            else
            {
                thisTilePath.PathConnectionScore = 7;

                if(currentScoreLeft == 17)
                {
                    left.MazeTilePath.PathConnectionScore = 2;
                }
                else if (currentScoreLeft == 22)
                {
                    left.MazeTilePath.PathConnectionScore = 29;
                }
                else if ((currentScoreLeft == 16 || currentScoreLeft == 21 || currentScoreLeft == 23 || currentScoreLeft == 31 || currentScoreLeft == 32 || currentScoreLeft == 33) && currentScoreRight != 25 && currentScoreRight != 26 && currentScoreRight != 31 && currentScoreRight != 33 && currentScoreRight != 34)
                {
                    thisTilePath.PathConnectionScore = 29;
                }
                else if(currentScoreLeft == 27)
                {
                    left.MazeTilePath.PathConnectionScore = 7;
                }

                if (currentScoreRight == 19)
                {
                    right.MazeTilePath.PathConnectionScore = 4;
                }
                else if (currentScoreRight == 22)
                {
                    right.MazeTilePath.PathConnectionScore = 27;
                }
                else if ((currentScoreRight == 16 || currentScoreRight == 25 || currentScoreRight == 26 || currentScoreRight == 31 || currentScoreRight == 33 || currentScoreRight == 34) && currentScoreLeft != 21 && currentScoreLeft != 23 && currentScoreLeft != 31 && currentScoreLeft != 32 && currentScoreLeft != 33)
                {
                    thisTilePath.PathConnectionScore = 27;
                }
                else if (currentScoreRight == 29)
                {
                    right.MazeTilePath.PathConnectionScore = 7;
                }
            }
        }

        else if(currentScoreThisTilePath == 23)
        {
            if (currentScoreRight == 16 || currentScoreRight == 25 || currentScoreRight == 26 || currentScoreRight == 31 || currentScoreRight == 33 || currentScoreRight == 34 ||
                currentScoreUp == 16 || currentScoreUp == 21 || currentScoreUp == 25 || currentScoreUp == 31 || currentScoreUp == 32 || currentScoreUp == 34)
            {

            }
            else
            {
                thisTilePath.PathConnectionScore = 8;
            
                if(currentScoreRight == 19)
                {
                    right.MazeTilePath.PathConnectionScore = 4;
                }
                else if (currentScoreRight == 22)
                {
                    right.MazeTilePath.PathConnectionScore = 27;
                }
                else if (currentScoreRight == 29)
                {
                    right.MazeTilePath.PathConnectionScore = 7;
                }

                if(currentScoreUp == 18)
                {
                    up.MazeTilePath.PathConnectionScore = 3;
                }
                else if (currentScoreUp == 24)
                {
                    up.MazeTilePath.PathConnectionScore = 30;
                }
                else if (currentScoreUp == 28)
                {
                    up.MazeTilePath.PathConnectionScore = 10;
                }
            }
        }

        else if (currentScoreThisTilePath == 24)
        {
            if ((currentScoreDown == 16 || currentScoreDown == 23 || currentScoreDown == 26 || currentScoreDown == 32 || currentScoreDown == 33 || currentScoreDown == 34) &&
                (currentScoreUp == 16 || currentScoreUp == 21 || currentScoreUp == 25 || currentScoreUp == 31 || currentScoreUp == 32 || currentScoreUp == 34))
            {

            }
            else
            {
                thisTilePath.PathConnectionScore = 10;

                if (currentScoreDown == 20)
                {
                    down.MazeTilePath.PathConnectionScore = 5;
                }
                else if (currentScoreDown == 24)
                {
                    down.MazeTilePath.PathConnectionScore = 28;
                }
                else if ((currentScoreDown == 16 || currentScoreDown == 23 || currentScoreDown == 26 || currentScoreDown == 32 || currentScoreDown == 33 || currentScoreDown == 34) && currentScoreUp != 21 && currentScoreUp != 25 && currentScoreUp != 31 && currentScoreUp != 32 && currentScoreUp != 34)
                {
                    thisTilePath.PathConnectionScore = 28;
                }
                else if (currentScoreDown == 30)
                {
                    down.MazeTilePath.PathConnectionScore = 10;
                }

                if (currentScoreUp == 18)
                {
                    up.MazeTilePath.PathConnectionScore = 3;
                }
                else if (currentScoreUp == 24)
                {
                    up.MazeTilePath.PathConnectionScore = 30;
                }
                else if ((currentScoreUp == 16 || currentScoreUp == 21 || currentScoreUp == 25 || currentScoreUp == 31 || currentScoreUp == 32 || currentScoreUp == 34) && currentScoreDown != 23 && currentScoreDown != 26 && currentScoreDown != 32 && currentScoreDown != 33 && currentScoreDown != 34)
                {
                    thisTilePath.PathConnectionScore = 30;
                }
                else if (currentScoreUp == 28)
                {
                    up.MazeTilePath.PathConnectionScore = 10;
                }
            }      
        }

        else if (currentScoreThisTilePath == 25)
        {
            if(currentScoreDown == 16 || currentScoreDown == 23 || currentScoreDown == 26 || currentScoreDown == 32 || currentScoreDown == 33 || currentScoreDown == 34 ||
                currentScoreLeft == 16 || currentScoreLeft == 21 || currentScoreLeft == 23 || currentScoreLeft == 31 || currentScoreLeft == 33 || currentScoreLeft == 34)
            {

            }
            else
            {
                thisTilePath.PathConnectionScore = 9;

                if(currentScoreLeft == 17)
                {
                    left.MazeTilePath.PathConnectionScore = 2;
                }
                else if (currentScoreLeft == 22)
                {
                    left.MazeTilePath.PathConnectionScore = 29;
                }
                else if (currentScoreLeft == 27)
                {
                    left.MazeTilePath.PathConnectionScore = 7;
                }

                if(currentScoreDown == 20)
                {
                    down.MazeTilePath.PathConnectionScore = 5;
                }
                else if (currentScoreDown == 24)
                {
                    down.MazeTilePath.PathConnectionScore = 28;
                }
                else if (currentScoreDown == 30)
                {
                    down.MazeTilePath.PathConnectionScore = 10;
                }
            }
        }

        else if (currentScoreThisTilePath == 26)
        {
            if (currentScoreLeft == 16 || currentScoreLeft == 21 || currentScoreLeft == 23 || currentScoreLeft == 31 || currentScoreLeft == 33 || currentScoreLeft == 34 ||
                currentScoreUp == 16 || currentScoreUp == 21 || currentScoreUp == 25 || currentScoreUp == 31 || currentScoreUp == 32 || currentScoreUp == 34)
            {

            }
            else
            {
                thisTilePath.PathConnectionScore = 11;

                if(currentScoreLeft == 17)
                {
                    left.MazeTilePath.PathConnectionScore = 2;
                }
                else if (currentScoreLeft == 22)
                {
                    left.MazeTilePath.PathConnectionScore = 29;
                }
                else if (currentScoreLeft == 27)
                {
                    left.MazeTilePath.PathConnectionScore = 7;
                }

                if(currentScoreUp == 18)
                {
                    up.MazeTilePath.PathConnectionScore = 3;
                }
                else if (currentScoreUp == 24)
                {
                    up.MazeTilePath.PathConnectionScore = 30;
                }
                else if (currentScoreUp == 28)
                {
                    up.MazeTilePath.PathConnectionScore = 10;
                }
            }
        }

        else if(currentScoreThisTilePath == 27)
        {
            if (currentScoreRight == 25 || currentScoreRight == 26 || currentScoreRight == 31 || currentScoreRight == 33 || currentScoreRight == 34)
            {
                if(currentScoreLeft == 22)
                {
                    left.MazeTilePath.PathConnectionScore = 29;
                }
                else if(currentScoreLeft == 27)
                {
                    left.MazeTilePath.PathConnectionScore = 7;
                }
            }
            else
            { 
                thisTilePath.PathConnectionScore = 7;
            
                if(currentScoreRight == 22)
                {
                    right.MazeTilePath.PathConnectionScore = 27;
                }
                else if(currentScoreRight == 29)
                {
                    right.MazeTilePath.PathConnectionScore = 7;
                }
            }
        }

        else if(currentScoreThisTilePath == 28)
        {
            if (currentScoreDown == 23 || currentScoreDown == 26 || currentScoreUp == 31 || currentScoreDown == 33 || currentScoreDown == 34)
            {
                if(currentScoreUp == 24)
                {
                    up.MazeTilePath.PathConnectionScore = 30;
                }
                else if(currentScoreUp == 28)
                {
                    up.MazeTilePath.PathConnectionScore = 10;
                }
            }
            else
            {
                thisTilePath.PathConnectionScore = 10;

                if (currentScoreDown == 24)
                {
                    down.MazeTilePath.PathConnectionScore = 28;
                }

                else if (currentScoreDown == 30)
                {
                    down.MazeTilePath.PathConnectionScore = 10;
                }
            }         
        }

        else if(currentScoreThisTilePath == 29)
        {
            if (currentScoreLeft == 21 || currentScoreLeft == 23 || currentScoreLeft == 31 || currentScoreLeft == 32 || currentScoreLeft == 33)
            {
                if(currentScoreRight == 22)
                {
                    right.MazeTilePath.PathConnectionScore = 27;
                }
                else if(currentScoreRight == 29)
                {
                    right.MazeTilePath.PathConnectionScore = 7;
                }
            }
            else
            {
                thisTilePath.PathConnectionScore = 7;

                if (currentScoreLeft == 22)
                {
                    left.MazeTilePath.PathConnectionScore = 29;
                }

                else if (currentScoreLeft == 27)
                {
                    left.MazeTilePath.PathConnectionScore = 7;
                }
            }    
        }

        else if(currentScoreThisTilePath == 30)
        {
            if (currentScoreUp == 21 || currentScoreUp == 25 || currentScoreUp == 31 || currentScoreUp == 32 || currentScoreUp == 33 || currentScoreUp == 34)
            {
                if(currentScoreDown == 24)
                {
                    down.MazeTilePath.PathConnectionScore = 28;
                }
                else if(currentScoreDown == 30)
                {
                    down.MazeTilePath.PathConnectionScore = 10;
                }
            }
            else
            {
                thisTilePath.PathConnectionScore = 10;
                if (currentScoreUp == 24)
                {
                    up.MazeTilePath.PathConnectionScore = 30;
                }
                else if(currentScoreUp == 28)
                {
                    up.MazeTilePath.PathConnectionScore = 10;
                }
            }
        }

        else if (currentScoreThisTilePath == 31)
        {
            if (currentScoreRight == 25 || currentScoreRight == 26 || currentScoreRight == 31 || currentScoreRight == 33 || currentScoreRight == 34 ||
                currentScoreDown == 23 || currentScoreDown == 26 || currentScoreDown == 32 || currentScoreDown == 33 || currentScoreDown == 34 ||
                currentScoreLeft == 21 || currentScoreLeft == 23 || currentScoreLeft == 31 || currentScoreLeft == 32 || currentScoreLeft == 33)
            {
                if(currentScoreRight == 4)
                {
                    right.MazeTilePath.PathConnectionScore = 19;
                }
                else if(currentScoreRight == 7)
                {
                    right.MazeTilePath.PathConnectionScore = 29;
                }
                else if(currentScoreRight == 27)
                {
                    right.MazeTilePath.PathConnectionScore = 22;
                }

                if(currentScoreDown == 5)
                {
                    down.MazeTilePath.PathConnectionScore = 20;
                }
                else if(currentScoreDown == 10)
                {
                    down.MazeTilePath.PathConnectionScore = 30;
                }
                else if(currentScoreDown == 28)
                {
                    down.MazeTilePath.PathConnectionScore = 24;
                }

                if(currentScoreLeft == 2)
                {
                    left.MazeTilePath.PathConnectionScore = 17;
                }
                else if(currentScoreLeft == 7)
                {
                    left.MazeTilePath.PathConnectionScore = 27;
                }
                else if(currentScoreLeft == 29)
                {
                    left.MazeTilePath.PathConnectionScore = 22;
                }
            }
            else
            {
                thisTilePath.PathConnectionScore = 12;

                if (currentScoreRight == 19)
                {
                    right.MazeTilePath.PathConnectionScore = 4;
                }
                else if (currentScoreRight == 22)
                {
                    right.MazeTilePath.PathConnectionScore = 27;
                }
                else if (currentScoreRight == 29)
                {
                    right.MazeTilePath.PathConnectionScore = 7;
                }

                if (currentScoreDown == 20)
                {
                    down.MazeTilePath.PathConnectionScore = 5;
                }
                else if (currentScoreDown == 24)
                {
                    down.MazeTilePath.PathConnectionScore = 28;
                }
                else if (currentScoreDown == 30)
                {
                    down.MazeTilePath.PathConnectionScore = 10;
                }

                if (currentScoreLeft == 17)
                {
                    left.MazeTilePath.PathConnectionScore = 2;
                }
                else if (currentScoreLeft == 22)
                {
                    left.MazeTilePath.PathConnectionScore = 29;
                }
                else if (currentScoreLeft == 27)
                {
                    left.MazeTilePath.PathConnectionScore = 7;
                }
            }
        }

        else if (currentScoreThisTilePath == 32)
        {
            if (currentScoreRight == 25 || currentScoreRight == 26 || currentScoreRight == 31 || currentScoreRight == 33 || currentScoreRight == 34 ||
                currentScoreDown == 23 || currentScoreDown == 26 || currentScoreDown == 32 || currentScoreDown == 33 || currentScoreDown == 34 ||
                currentScoreUp == 21 || currentScoreUp == 25 || currentScoreUp == 32 || currentScoreUp == 33 || currentScoreUp == 34)
            {
                if (currentScoreRight == 4)
                {
                    right.MazeTilePath.PathConnectionScore = 19;
                }
                else if (currentScoreRight == 7)
                {
                    right.MazeTilePath.PathConnectionScore = 29;
                }
                else if (currentScoreRight == 27)
                {
                    right.MazeTilePath.PathConnectionScore = 22;
                }

                if (currentScoreDown == 5)
                {
                    down.MazeTilePath.PathConnectionScore = 20;
                }
                else if (currentScoreDown == 10)
                {
                    down.MazeTilePath.PathConnectionScore = 30;
                }
                else if (currentScoreDown == 28)
                {
                    down.MazeTilePath.PathConnectionScore = 24;
                }

                if (currentScoreUp == 3)
                {
                    up.MazeTilePath.PathConnectionScore = 18;
                }
                else if (currentScoreUp == 10)
                {
                    up.MazeTilePath.PathConnectionScore = 28;
                }
                else if (currentScoreUp == 30)
                {
                    up.MazeTilePath.PathConnectionScore = 24;
                }
            }
            else
            {
                thisTilePath.PathConnectionScore = 13;

                if (currentScoreRight == 19)
                {
                    right.MazeTilePath.PathConnectionScore = 4;
                }
                else if (currentScoreRight == 22)
                {
                    right.MazeTilePath.PathConnectionScore = 27;
                }
                else if (currentScoreRight == 29)
                {
                    right.MazeTilePath.PathConnectionScore = 7;
                }
                
                if (currentScoreDown == 20)
                {
                    down.MazeTilePath.PathConnectionScore = 5;
                }
                else if (currentScoreDown == 24)
                {
                    down.MazeTilePath.PathConnectionScore = 28;
                }
                else if (currentScoreDown == 30)
                {
                    down.MazeTilePath.PathConnectionScore = 10;
                }

                if (currentScoreUp == 18)
                {
                    up.MazeTilePath.PathConnectionScore = 3;
                }
                else if (currentScoreUp == 24)
                {
                    up.MazeTilePath.PathConnectionScore = 30;
                }
                else if (currentScoreUp == 28)
                {
                    up.MazeTilePath.PathConnectionScore = 10;
                }
            }
        }

        else if (currentScoreThisTilePath == 33)
        {
            if (currentScoreRight == 25 || currentScoreRight == 26 || currentScoreRight == 31 || currentScoreRight == 33 || currentScoreRight == 34 ||
                currentScoreLeft == 21 || currentScoreLeft == 2 || currentScoreLeft == 31 || currentScoreLeft == 32 || currentScoreLeft == 33 ||
                currentScoreUp == 21 || currentScoreUp == 25 || currentScoreUp == 32 || currentScoreUp == 33 || currentScoreUp == 34)
            {
                if(currentScoreRight == 4)
                {
                    right.MazeTilePath.PathConnectionScore = 19;
                }
                else if (currentScoreRight == 7)
                {
                    right.MazeTilePath.PathConnectionScore = 29;
                }
                else if (currentScoreRight == 27)
                {
                    right.MazeTilePath.PathConnectionScore = 22;
                }

                if(currentScoreLeft == 2)
                {
                    left.MazeTilePath.PathConnectionScore = 17;
                }
                else if (currentScoreLeft == 7)
                {
                    left.MazeTilePath.PathConnectionScore = 27;
                }
                else if (currentScoreLeft == 29)
                {
                    left.MazeTilePath.PathConnectionScore = 22;
                }

                if(currentScoreUp == 3)
                {
                    up.MazeTilePath.PathConnectionScore = 18;
                }
                else if (currentScoreUp == 10)
                {
                    up.MazeTilePath.PathConnectionScore = 28;
                }
                else if (currentScoreUp == 30)
                {
                    up.MazeTilePath.PathConnectionScore = 24;
                }
            }
            else
            {
                thisTilePath.PathConnectionScore = 14;

                if (currentScoreRight == 19)
                {
                    right.MazeTilePath.PathConnectionScore = 4;
                }
                else if (currentScoreRight == 22)
                {
                    right.MazeTilePath.PathConnectionScore = 27;
                }
                else if (currentScoreRight == 29)
                {
                    right.MazeTilePath.PathConnectionScore = 7;
                }

                if (currentScoreLeft == 17)
                {
                    left.MazeTilePath.PathConnectionScore = 2;
                }
                else if (currentScoreLeft == 22)
                {
                    left.MazeTilePath.PathConnectionScore = 29;
                }
                else if (currentScoreLeft == 27)
                {
                    left.MazeTilePath.PathConnectionScore = 7;
                }

                if (currentScoreUp == 18)
                {
                    up.MazeTilePath.PathConnectionScore = 3;
                }
                else if (currentScoreUp == 24)
                {
                    up.MazeTilePath.PathConnectionScore = 30;
                }
                else if (currentScoreUp == 28)
                {
                    up.MazeTilePath.PathConnectionScore = 10;
                }
            }
        }

        else if (currentScoreThisTilePath == 34)
        {
            if (currentScoreDown == 23 || currentScoreDown == 26 || currentScoreDown == 32 || currentScoreDown == 33 || currentScoreDown == 34 ||
                currentScoreLeft == 21 || currentScoreLeft == 23 || currentScoreLeft == 31 || currentScoreLeft == 32 || currentScoreLeft == 33 ||
                currentScoreUp == 21 || currentScoreUp == 25 || currentScoreUp == 32 || currentScoreUp == 33 || currentScoreUp == 34)
            {
                if (currentScoreDown == 5)
                {
                    down.MazeTilePath.PathConnectionScore = 20;
                }
                else if (currentScoreDown == 10)
                {
                    down.MazeTilePath.PathConnectionScore = 30;
                }
                else if (currentScoreDown == 28)
                {
                    down.MazeTilePath.PathConnectionScore = 24;
                }

                if (currentScoreLeft == 2)
                {
                    left.MazeTilePath.PathConnectionScore = 17;
                }
                else if (currentScoreLeft == 7)
                {
                    left.MazeTilePath.PathConnectionScore = 27;
                }
                else if (currentScoreLeft == 29)
                {
                    left.MazeTilePath.PathConnectionScore = 22;
                }

                if (currentScoreUp == 3)
                {
                    up.MazeTilePath.PathConnectionScore = 18;
                }
                else if (currentScoreUp == 10)
                {
                    up.MazeTilePath.PathConnectionScore = 28;
                }
                else if (currentScoreUp == 30)
                {
                    up.MazeTilePath.PathConnectionScore = 24;
                }
            }
            else
            {
                thisTilePath.PathConnectionScore = 15;

                if (currentScoreDown == 20)
                {
                    down.MazeTilePath.PathConnectionScore = 5;
                }
                else if (currentScoreDown == 24)
                {
                    down.MazeTilePath.PathConnectionScore = 28;
                }
                else if (currentScoreDown == 30)
                {
                    down.MazeTilePath.PathConnectionScore = 10;
                }

                if (currentScoreLeft == 17)
                {
                    left.MazeTilePath.PathConnectionScore = 2;
                }
                else if (currentScoreLeft == 22)
                {
                    left.MazeTilePath.PathConnectionScore = 29;
                }
                else if (currentScoreLeft == 27)
                {
                    left.MazeTilePath.PathConnectionScore = 7;
                }

                if (currentScoreUp == 18)
                {
                    up.MazeTilePath.PathConnectionScore = 3;
                }
                else if (currentScoreUp == 24)
                {
                    up.MazeTilePath.PathConnectionScore = 30;
                }
                else if (currentScoreUp == 28)
                {
                    up.MazeTilePath.PathConnectionScore = 10;
                }
            }
        }

        if (currentScoreThisTilePath != thisTilePath.PathConnectionScore)
            updatedConnections.Add(thisTilePath);
        if (right.MazeTilePath && currentScoreRight != right.MazeTilePath.PathConnectionScore)
            updatedConnections.Add(right.MazeTilePath);
        if (down.MazeTilePath && currentScoreDown != down.MazeTilePath.PathConnectionScore)
            updatedConnections.Add(down.MazeTilePath);
        if (left.MazeTilePath && currentScoreLeft != left.MazeTilePath.PathConnectionScore)
            updatedConnections.Add(left.MazeTilePath);
        if (up.MazeTilePath && currentScoreUp != up.MazeTilePath.PathConnectionScore)
            updatedConnections.Add(up.MazeTilePath);

        return updatedConnections;
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