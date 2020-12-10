using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct TileModifierConnectionInfo<T> where T : MonoBehaviour, ITileConnectable
{
    public Direction Direction;
    public bool HasConnection;
    public T TileModifier;

    public TileModifierConnectionInfo(Direction direction, bool hasConnection = false, T tileModifier = null)
    {
        Direction = direction;
        HasConnection = hasConnection;
        TileModifier = tileModifier;
    }
}

public class NeighbourTileCalculator
{
    public static readonly int ConnectionOnAllSidesScore = 16;

    public static int MapNeighbourPlayerMarkEndsOfTile(Tile tile)
    {
        bool hasMarkRight = false;
        bool hasMarkDown = false;
        bool hasMarkLeft = false;
        bool hasMarkUp = false;

        foreach (KeyValuePair<ObjectDirection, Tile> neighbour in tile.Neighbours)
        {
            if (neighbour.Value.PlayerMark == null || neighbour.Value.PlayerMark.Owner == PlayerMarkOwner.None) continue;

            if (neighbour.Key == ObjectDirection.Right)
            {
                hasMarkRight = true;
            }
            else if (neighbour.Key == ObjectDirection.Down)
            {
                hasMarkDown = true;
            }
            else if (neighbour.Key == ObjectDirection.Left)
            {
                hasMarkLeft = true;
            }
            else if (neighbour.Key == ObjectDirection.Up)
            {
                hasMarkUp = true;
            }
        }
        return Calculate16TileMapConnectionScore(hasMarkRight, hasMarkDown, hasMarkLeft, hasMarkUp);
    }

    public static int MapNeighbourPathsOfTile(Tile tile, MazeTilePathType pathType)
    {
        Logger.Log($"---------Map neighbours of {tile.GridLocation.X},{tile.GridLocation.Y}--------");
        TileModifierConnectionInfo<MazeTilePath> pathRight = new TileModifierConnectionInfo<MazeTilePath>(Direction.Right);
        TileModifierConnectionInfo<MazeTilePath> pathDown = new TileModifierConnectionInfo<MazeTilePath>(Direction.Down);
        TileModifierConnectionInfo<MazeTilePath> pathLeft = new TileModifierConnectionInfo<MazeTilePath>(Direction.Left);
        TileModifierConnectionInfo<MazeTilePath> pathUp = new TileModifierConnectionInfo<MazeTilePath>(Direction.Up);

        if (!EditorManager.InEditor)
        {
            Logger.Error("MapNeighbourPathsOfTile was not called from the editor");
            return -1;
        }

        foreach (KeyValuePair<ObjectDirection, Tile> neighbour in tile.Neighbours)
        {
            Logger.Warning($"Neighbour at {neighbour.Value.GridLocation.X},{neighbour.Value.GridLocation.Y} is {neighbour.Key} of {tile.GridLocation.X},{tile.GridLocation.Y}");
            
            MazeTilePath tilePath = neighbour.Value.TryGetTilePath();

            if (tilePath == null || tilePath.MazeTilePathType != pathType)
            {
                continue;
            }

            if (neighbour.Key == ObjectDirection.Right)
            {
                pathRight.HasConnection = true;
                pathRight.TileModifier = tilePath;
            }
            else if (neighbour.Key == ObjectDirection.Down)
            {
                pathDown.HasConnection = true;
                pathDown.TileModifier = tilePath;
            }
            else if (neighbour.Key == ObjectDirection.Left)
            {
                pathLeft.HasConnection = true;
                pathLeft.TileModifier = tilePath;
            }
            else if (neighbour.Key == ObjectDirection.Up)
            {
                pathUp.HasConnection = true;
                pathUp.TileModifier = tilePath;
            }
        }

        return CalculateTileConnectionScore(pathRight, pathDown, pathLeft, pathUp);
    }

    public static int MapNeighbourObstaclesOfTile(Tile tile, ObstacleType obstacleType, bool isDoor)
    {
        Logger.Log($"Map neighbours of {tile.GridLocation.X},{tile.GridLocation.Y}");
        TileModifierConnectionInfo<TileObstacle> obstacleRight = new TileModifierConnectionInfo<TileObstacle>(Direction.Right);
        TileModifierConnectionInfo<TileObstacle> obstacleDown = new TileModifierConnectionInfo<TileObstacle>(Direction.Down);
        TileModifierConnectionInfo<TileObstacle> obstacleLeft = new TileModifierConnectionInfo<TileObstacle>(Direction.Left);
        TileModifierConnectionInfo<TileObstacle> obstacleUp = new TileModifierConnectionInfo<TileObstacle>(Direction.Up);

        if (!EditorManager.InEditor)
        {
            Logger.Error("MapNeighbourObstaclesOfTile was not called from the editor");
            return -1;
        }

        foreach (KeyValuePair<ObjectDirection, Tile> neighbour in tile.Neighbours)
        {
            Logger.Warning($"Neighbour at {neighbour.Value.GridLocation.X},{neighbour.Value.GridLocation.Y} is {neighbour.Key} of {tile.GridLocation.X},{tile.GridLocation.Y}");
            
            TileObstacle tileObstacle = neighbour.Value.TryGetTileObstacle();

            if (tileObstacle == null || tileObstacle.ObstacleType != obstacleType)
            {
                continue;
            }

            if (neighbour.Key == ObjectDirection.Right)
            {
                obstacleRight.HasConnection = true;
                obstacleRight.TileModifier = tileObstacle;
            }
            else if (neighbour.Key == ObjectDirection.Down)
            {
                obstacleDown.HasConnection = true;
                obstacleDown.TileModifier = tileObstacle;
            }
            else if (neighbour.Key == ObjectDirection.Left)
            {
                obstacleLeft.HasConnection = true;
                obstacleLeft.TileModifier = tileObstacle;
            }
            else if (neighbour.Key == ObjectDirection.Up)
            {
                obstacleUp.HasConnection = true;
                obstacleUp.TileModifier = tileObstacle;
            }
        }

        if (isDoor)
        {
            return CalculateDoorConnectionScore(obstacleRight, obstacleDown, obstacleLeft, obstacleUp);
        }
        return CalculateTileConnectionScore(obstacleRight, obstacleDown, obstacleLeft, obstacleUp);
    }

    public static List<T> GetUpdatedTileModifiersForVariation<T>(Tile tile, T thisMazeTileModifier, string modifierSubtype) where T : MonoBehaviour, ITileConnectable
    {
        Logger.Log($"---------Map neighbours of {tile.GridLocation.X},{tile.GridLocation.Y}--------");
        TileModifierConnectionInfo<T> connectionRight = new TileModifierConnectionInfo<T>(Direction.Right);
        TileModifierConnectionInfo<T> connectionDown = new TileModifierConnectionInfo<T>(Direction.Down);
        TileModifierConnectionInfo<T> connectionLeft = new TileModifierConnectionInfo<T>(Direction.Left);
        TileModifierConnectionInfo<T> connectionUp = new TileModifierConnectionInfo<T>(Direction.Up);

        foreach (KeyValuePair<ObjectDirection, Tile> neighbour in tile.Neighbours)
        {
            Logger.Warning($"Neighbour at {neighbour.Value.GridLocation.X},{neighbour.Value.GridLocation.Y} is {neighbour.Key} of {tile.GridLocation.X},{tile.GridLocation.Y}");

            T connectedModifier;
            Type modifierType = typeof(T);

            if (modifierType is IMazeTileAttribute)
            {
                connectedModifier = (T)neighbour.Value.MazeTileAttributes.FirstOrDefault(attribute => attribute is T);
            } 
            else
            {
                connectedModifier = (T)neighbour.Value.MazeTileBackgrounds.FirstOrDefault(attribute => attribute is T);
            }

            if (connectedModifier == null || connectedModifier.GetSubtypeAsString() != modifierSubtype)
            {
                continue;
            }

            if (neighbour.Key == ObjectDirection.Right)
            {
                connectionRight.HasConnection = true;
                connectionRight.TileModifier = connectedModifier;
            }
            else if (neighbour.Key == ObjectDirection.Down)
            {
                
                connectionDown.HasConnection = true;
                connectionDown.TileModifier = connectedModifier;

            }
            else if (neighbour.Key == ObjectDirection.Left)
            {
                connectionLeft.HasConnection = true;
                connectionLeft.TileModifier = connectedModifier;
            }
            else if (neighbour.Key == ObjectDirection.Up)
            {
                connectionUp.HasConnection = true;
                connectionUp.TileModifier = connectedModifier;
            }
        }

        return CalculateTileConnectionScoreForVariations(thisMazeTileModifier, connectionRight, connectionDown, connectionLeft, connectionUp);
    }

    private static int CalculateTileConnectionScore<T>(TileModifierConnectionInfo<T> right, TileModifierConnectionInfo<T> down, TileModifierConnectionInfo<T> left, TileModifierConnectionInfo<T> up) where T : MonoBehaviour, ITileConnectable
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
            if (left.HasConnection)
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

    private static List<T> CalculateTileConnectionScoreForVariations<T>(T thisMazeTileAttribute, TileModifierConnectionInfo<T> connectionRight, TileModifierConnectionInfo<T> connectionDown, TileModifierConnectionInfo<T> connectionLeft, TileModifierConnectionInfo<T> connectionUp) where T : MonoBehaviour, ITileConnectable
    {
        List<T> updatedConnections = new List<T>();

        int currentScoreThisTilePath = thisMazeTileAttribute.ConnectionScore;
        int currentScoreRight = connectionRight.TileModifier ? connectionRight.TileModifier.ConnectionScore : -1;
        int currentScoreDown = connectionDown.TileModifier ? connectionDown.TileModifier.ConnectionScore : -1;
        int currentScoreLeft = connectionLeft.TileModifier ? connectionLeft.TileModifier.ConnectionScore : -1;
        int currentScoreUp = connectionUp.TileModifier ? connectionUp.TileModifier.ConnectionScore : -1;

        if (currentScoreThisTilePath == 2)
        {
            if (currentScoreRight == 22)
            {
                connectionRight.TileModifier.ConnectionScore = 27;
            }
            else if (currentScoreRight == 29)
            {
                connectionRight.TileModifier.ConnectionScore = 7;
            }
        }

        if (currentScoreThisTilePath == 3)
        {
            if (currentScoreDown == 24)
            {
                connectionDown.TileModifier.ConnectionScore = 28;
            }
            else if (currentScoreDown == 30)
            {
                connectionDown.TileModifier.ConnectionScore = 10;
            }
        }

        if (currentScoreThisTilePath == 4)
        {
            if (currentScoreLeft == 22)
            {
                connectionLeft.TileModifier.ConnectionScore = 29;
            }
            else if (currentScoreLeft == 27)
            {
                connectionLeft.TileModifier.ConnectionScore = 7;
            }
        }

        if (currentScoreThisTilePath == 5)
        {
            if (currentScoreUp == 24)
            {
                connectionUp.TileModifier.ConnectionScore = 30;
            }
            else if (currentScoreUp == 28)
            {
                connectionUp.TileModifier.ConnectionScore = 10;
            }
        }

        if (currentScoreThisTilePath == 7)
        {
            if (currentScoreRight == 19)
            {
                connectionRight.TileModifier.ConnectionScore = 4;
            }
            else if (currentScoreRight == 22)
            {
                connectionRight.TileModifier.ConnectionScore = 27;
            }
            else if (currentScoreRight == 29)
            {
                connectionRight.TileModifier.ConnectionScore = 7;
            }

            if (currentScoreLeft == 17)
            {
                connectionLeft.TileModifier.ConnectionScore = 2;
            }
            else if (currentScoreLeft == 22)
            {
                connectionLeft.TileModifier.ConnectionScore = 29;
            }
            else if (currentScoreLeft == 27)
            {
                connectionLeft.TileModifier.ConnectionScore = 7;
            }
        }

        if (currentScoreThisTilePath == 10)
        {
            if (currentScoreDown == 20)
            {
                connectionDown.TileModifier.ConnectionScore = 5;
            }
            else if (currentScoreDown == 24)
            {
                connectionDown.TileModifier.ConnectionScore = 28;
            }
            else if (currentScoreDown == 30)
            {
                connectionDown.TileModifier.ConnectionScore = 10;
            }

            if (currentScoreUp == 18)
            {
                connectionUp.TileModifier.ConnectionScore = 3;
            }
            else if (currentScoreUp == 24)
            {
                connectionUp.TileModifier.ConnectionScore = 30;
            }
            else if (currentScoreUp == 28)
            {
                connectionUp.TileModifier.ConnectionScore = 10;
            }
        }

        if (currentScoreThisTilePath == 16)
        {
            if (currentScoreRight == 7)
            {
                connectionRight.TileModifier.ConnectionScore = 29;
            }
            else if (currentScoreLeft == 27)
            {
                connectionRight.TileModifier.ConnectionScore = 22;
            }

            if (currentScoreDown == 10)
            {
                connectionDown.TileModifier.ConnectionScore = 30;
            }
            else if (currentScoreDown == 28)
            {
                connectionDown.TileModifier.ConnectionScore = 24;
            }

            if (currentScoreLeft == 7)
            {
                connectionLeft.TileModifier.ConnectionScore = 27;
            }
            else if (currentScoreLeft == 27)
            {
                connectionLeft.TileModifier.ConnectionScore = 22;
            }

            if (currentScoreUp == 10)
            {
                connectionUp.TileModifier.ConnectionScore = 28;
            }
            else if (currentScoreUp == 30)
            {
                connectionUp.TileModifier.ConnectionScore = 24;
            }
        }

        if (currentScoreThisTilePath == 17)
        {
            if (currentScoreRight == 16 || currentScoreRight == 25 || currentScoreRight == 26 || currentScoreRight == 31 || currentScoreRight == 32 || currentScoreRight == 33 || currentScoreRight == 34)
            {

            }
            else
            {
                thisMazeTileAttribute.ConnectionScore = 2;

                if (currentScoreRight == 19)
                {
                    connectionRight.TileModifier.ConnectionScore = 4;
                }
                else if (currentScoreRight == 22)
                {
                    connectionRight.TileModifier.ConnectionScore = 27;
                }
                else if (currentScoreRight == 29)
                {
                    connectionRight.TileModifier.ConnectionScore = 27;
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
                thisMazeTileAttribute.ConnectionScore = 3;

                if (currentScoreDown == 20)
                {
                    connectionDown.TileModifier.ConnectionScore = 5;
                }
                else if (currentScoreDown == 24)
                {
                    connectionDown.TileModifier.ConnectionScore = 28;
                }
                else if (currentScoreDown == 30)
                {
                    connectionDown.TileModifier.ConnectionScore = 10;
                }
            }
        }

        if (currentScoreThisTilePath == 19)
        {
            if (currentScoreLeft == 16 || currentScoreLeft == 21 || currentScoreLeft == 23 || currentScoreLeft == 31 || currentScoreLeft == 32 || currentScoreLeft == 33)
            {

            }
            else
            {
                thisMazeTileAttribute.ConnectionScore = 4;

                if (currentScoreLeft == 17)
                {
                    connectionLeft.TileModifier.ConnectionScore = 2;
                }
                else if (currentScoreLeft == 22)
                {
                    connectionLeft.TileModifier.ConnectionScore = 29;
                }
                else if (currentScoreLeft == 27)
                {
                    connectionLeft.TileModifier.ConnectionScore = 7;
                }
            }
        }

        else if (currentScoreThisTilePath == 20)
        {
            if (currentScoreUp == 16 || currentScoreUp == 21 || currentScoreUp == 25 || currentScoreUp == 31 || currentScoreUp == 32 || currentScoreUp == 34)
            {

            }
            else
            {
                thisMazeTileAttribute.ConnectionScore = 5;

                if (currentScoreUp == 18)
                {
                    connectionUp.TileModifier.ConnectionScore = 5;
                }
                else if (currentScoreUp == 24)
                {
                    connectionUp.TileModifier.ConnectionScore = 30;
                }
                else if (currentScoreUp == 28)
                {
                    connectionUp.TileModifier.ConnectionScore = 10;
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
                thisMazeTileAttribute.ConnectionScore = 6;

                if (currentScoreRight == 19)
                {
                    connectionRight.TileModifier.ConnectionScore = 4;
                }
                else if (currentScoreRight == 22)
                {
                    connectionRight.TileModifier.ConnectionScore = 27;
                }
                else if (currentScoreRight == 29)
                {
                    connectionRight.TileModifier.ConnectionScore = 7;
                }

                if (currentScoreDown == 20)
                {
                    connectionDown.TileModifier.ConnectionScore = 5;
                }
                else if (currentScoreDown == 24)
                {
                    connectionDown.TileModifier.ConnectionScore = 28;
                }
                else if (currentScoreDown == 30)
                {
                    connectionDown.TileModifier.ConnectionScore = 10;
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
                thisMazeTileAttribute.ConnectionScore = 7;

                if (currentScoreLeft == 17)
                {
                    connectionLeft.TileModifier.ConnectionScore = 2;
                }
                else if (currentScoreLeft == 22)
                {
                    connectionLeft.TileModifier.ConnectionScore = 29;
                }
                else if ((currentScoreLeft == 16 || currentScoreLeft == 21 || currentScoreLeft == 23 || currentScoreLeft == 31 || currentScoreLeft == 32 || currentScoreLeft == 33) && currentScoreRight != 25 && currentScoreRight != 26 && currentScoreRight != 31 && currentScoreRight != 33 && currentScoreRight != 34)
                {
                    thisMazeTileAttribute.ConnectionScore = 29;
                }
                else if (currentScoreLeft == 27)
                {
                    connectionLeft.TileModifier.ConnectionScore = 7;
                }

                if (currentScoreRight == 19)
                {
                    connectionRight.TileModifier.ConnectionScore = 4;
                }
                else if (currentScoreRight == 22)
                {
                    connectionRight.TileModifier.ConnectionScore = 27;
                }
                else if ((currentScoreRight == 16 || currentScoreRight == 25 || currentScoreRight == 26 || currentScoreRight == 31 || currentScoreRight == 33 || currentScoreRight == 34) && currentScoreLeft != 21 && currentScoreLeft != 23 && currentScoreLeft != 31 && currentScoreLeft != 32 && currentScoreLeft != 33)
                {
                    thisMazeTileAttribute.ConnectionScore = 27;
                }
                else if (currentScoreRight == 29)
                {
                    connectionRight.TileModifier.ConnectionScore = 7;
                }
            }
        }

        else if (currentScoreThisTilePath == 23)
        {
            if (currentScoreRight == 16 || currentScoreRight == 25 || currentScoreRight == 26 || currentScoreRight == 31 || currentScoreRight == 33 || currentScoreRight == 34 ||
                currentScoreUp == 16 || currentScoreUp == 21 || currentScoreUp == 25 || currentScoreUp == 31 || currentScoreUp == 32 || currentScoreUp == 34)
            {

            }
            else
            {
                thisMazeTileAttribute.ConnectionScore = 8;

                if (currentScoreRight == 19)
                {
                    connectionRight.TileModifier.ConnectionScore = 4;
                }
                else if (currentScoreRight == 22)
                {
                    connectionRight.TileModifier.ConnectionScore = 27;
                }
                else if (currentScoreRight == 29)
                {
                    connectionRight.TileModifier.ConnectionScore = 7;
                }

                if (currentScoreUp == 18)
                {
                    connectionUp.TileModifier.ConnectionScore = 3;
                }
                else if (currentScoreUp == 24)
                {
                    connectionUp.TileModifier.ConnectionScore = 30;
                }
                else if (currentScoreUp == 28)
                {
                    connectionUp.TileModifier.ConnectionScore = 10;
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
                thisMazeTileAttribute.ConnectionScore = 10;

                if (currentScoreDown == 20)
                {
                    connectionDown.TileModifier.ConnectionScore = 5;
                }
                else if (currentScoreDown == 24)
                {
                    connectionDown.TileModifier.ConnectionScore = 28;
                }
                else if ((currentScoreDown == 16 || currentScoreDown == 23 || currentScoreDown == 26 || currentScoreDown == 32 || currentScoreDown == 33 || currentScoreDown == 34) && currentScoreUp != 21 && currentScoreUp != 25 && currentScoreUp != 31 && currentScoreUp != 32 && currentScoreUp != 34)
                {
                    thisMazeTileAttribute.ConnectionScore = 28;
                }
                else if (currentScoreDown == 30)
                {
                    connectionDown.TileModifier.ConnectionScore = 10;
                }

                if (currentScoreUp == 18)
                {
                    connectionUp.TileModifier.ConnectionScore = 3;
                }
                else if (currentScoreUp == 24)
                {
                    connectionUp.TileModifier.ConnectionScore = 30;
                }
                else if ((currentScoreUp == 16 || currentScoreUp == 21 || currentScoreUp == 25 || currentScoreUp == 31 || currentScoreUp == 32 || currentScoreUp == 34) && currentScoreDown != 23 && currentScoreDown != 26 && currentScoreDown != 32 && currentScoreDown != 33 && currentScoreDown != 34)
                {
                    thisMazeTileAttribute.ConnectionScore = 30;
                }
                else if (currentScoreUp == 28)
                {
                    connectionUp.TileModifier.ConnectionScore = 10;
                }
            }
        }

        else if (currentScoreThisTilePath == 25)
        {
            if (currentScoreDown == 16 || currentScoreDown == 23 || currentScoreDown == 26 || currentScoreDown == 32 || currentScoreDown == 33 || currentScoreDown == 34 ||
                currentScoreLeft == 16 || currentScoreLeft == 21 || currentScoreLeft == 23 || currentScoreLeft == 31 || currentScoreLeft == 33 || currentScoreLeft == 34)
            {

            }
            else
            {
                thisMazeTileAttribute.ConnectionScore = 9;

                if (currentScoreLeft == 17)
                {
                    connectionLeft.TileModifier.ConnectionScore = 2;
                }
                else if (currentScoreLeft == 22)
                {
                    connectionLeft.TileModifier.ConnectionScore = 29;
                }
                else if (currentScoreLeft == 27)
                {
                    connectionLeft.TileModifier.ConnectionScore = 7;
                }

                if (currentScoreDown == 20)
                {
                    connectionDown.TileModifier.ConnectionScore = 5;
                }
                else if (currentScoreDown == 24)
                {
                    connectionDown.TileModifier.ConnectionScore = 28;
                }
                else if (currentScoreDown == 30)
                {
                    connectionDown.TileModifier.ConnectionScore = 10;
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
                thisMazeTileAttribute.ConnectionScore = 11;

                if (currentScoreLeft == 17)
                {
                    connectionLeft.TileModifier.ConnectionScore = 2;
                }
                else if (currentScoreLeft == 22)
                {
                    connectionLeft.TileModifier.ConnectionScore = 29;
                }
                else if (currentScoreLeft == 27)
                {
                    connectionLeft.TileModifier.ConnectionScore = 7;
                }

                if (currentScoreUp == 18)
                {
                    connectionUp.TileModifier.ConnectionScore = 3;
                }
                else if (currentScoreUp == 24)
                {
                    connectionUp.TileModifier.ConnectionScore = 30;
                }
                else if (currentScoreUp == 28)
                {
                    connectionUp.TileModifier.ConnectionScore = 10;
                }
            }
        }

        else if (currentScoreThisTilePath == 27)
        {
            if (currentScoreRight == 25 || currentScoreRight == 26 || currentScoreRight == 31 || currentScoreRight == 33 || currentScoreRight == 34)
            {
                if (currentScoreLeft == 22)
                {
                    connectionLeft.TileModifier.ConnectionScore = 29;
                }
                else if (currentScoreLeft == 27)
                {
                    connectionLeft.TileModifier.ConnectionScore = 7;
                }
            }
            else
            {
                thisMazeTileAttribute.ConnectionScore = 7;

                if (currentScoreRight == 22)
                {
                    connectionRight.TileModifier.ConnectionScore = 27;
                }
                else if (currentScoreRight == 29)
                {
                    connectionRight.TileModifier.ConnectionScore = 7;
                }
            }
        }

        else if (currentScoreThisTilePath == 28)
        {
            if (currentScoreDown == 23 || currentScoreDown == 26 || currentScoreUp == 31 || currentScoreDown == 33 || currentScoreDown == 34)
            {
                if (currentScoreUp == 24)
                {
                    connectionUp.TileModifier.ConnectionScore = 30;
                }
                else if (currentScoreUp == 28)
                {
                    connectionUp.TileModifier.ConnectionScore = 10;
                }
            }
            else
            {
                thisMazeTileAttribute.ConnectionScore = 10;

                if (currentScoreDown == 24)
                {
                    connectionDown.TileModifier.ConnectionScore = 28;
                }

                else if (currentScoreDown == 30)
                {
                    connectionDown.TileModifier.ConnectionScore = 10;
                }
            }
        }

        else if (currentScoreThisTilePath == 29)
        {
            if (currentScoreLeft == 21 || currentScoreLeft == 23 || currentScoreLeft == 31 || currentScoreLeft == 32 || currentScoreLeft == 33)
            {
                if (currentScoreRight == 22)
                {
                    connectionRight.TileModifier.ConnectionScore = 27;
                }
                else if (currentScoreRight == 29)
                {
                    connectionRight.TileModifier.ConnectionScore = 7;
                }
            }
            else
            {
                thisMazeTileAttribute.ConnectionScore = 7;

                if (currentScoreLeft == 22)
                {
                    connectionLeft.TileModifier.ConnectionScore = 29;
                }

                else if (currentScoreLeft == 27)
                {
                    connectionLeft.TileModifier.ConnectionScore = 7;
                }
            }
        }

        else if (currentScoreThisTilePath == 30)
        {
            if (currentScoreUp == 21 || currentScoreUp == 25 || currentScoreUp == 31 || currentScoreUp == 32 || currentScoreUp == 33 || currentScoreUp == 34)
            {
                if (currentScoreDown == 24)
                {
                    connectionDown.TileModifier.ConnectionScore = 28;
                }
                else if (currentScoreDown == 30)
                {
                    connectionDown.TileModifier.ConnectionScore = 10;
                }
            }
            else
            {
                thisMazeTileAttribute.ConnectionScore = 10;
                if (currentScoreUp == 24)
                {
                    connectionUp.TileModifier.ConnectionScore = 30;
                }
                else if (currentScoreUp == 28)
                {
                    connectionUp.TileModifier.ConnectionScore = 10;
                }
            }
        }

        else if (currentScoreThisTilePath == 31)
        {
            if (currentScoreRight == 25 || currentScoreRight == 26 || currentScoreRight == 31 || currentScoreRight == 33 || currentScoreRight == 34 ||
                currentScoreDown == 23 || currentScoreDown == 26 || currentScoreDown == 32 || currentScoreDown == 33 || currentScoreDown == 34 ||
                currentScoreLeft == 21 || currentScoreLeft == 23 || currentScoreLeft == 31 || currentScoreLeft == 32 || currentScoreLeft == 33)
            {
                if (currentScoreRight == 4)
                {
                    connectionRight.TileModifier.ConnectionScore = 19;
                }
                else if (currentScoreRight == 7)
                {
                    connectionRight.TileModifier.ConnectionScore = 29;
                }
                else if (currentScoreRight == 27)
                {
                    connectionRight.TileModifier.ConnectionScore = 22;
                }

                if (currentScoreDown == 5)
                {
                    connectionDown.TileModifier.ConnectionScore = 20;
                }
                else if (currentScoreDown == 10)
                {
                    connectionDown.TileModifier.ConnectionScore = 30;
                }
                else if (currentScoreDown == 28)
                {
                    connectionDown.TileModifier.ConnectionScore = 24;
                }

                if (currentScoreLeft == 2)
                {
                    connectionLeft.TileModifier.ConnectionScore = 17;
                }
                else if (currentScoreLeft == 7)
                {
                    connectionLeft.TileModifier.ConnectionScore = 27;
                }
                else if (currentScoreLeft == 29)
                {
                    connectionLeft.TileModifier.ConnectionScore = 22;
                }
            }
            else
            {
                thisMazeTileAttribute.ConnectionScore = 12;

                if (currentScoreRight == 19)
                {
                    connectionRight.TileModifier.ConnectionScore = 4;
                }
                else if (currentScoreRight == 22)
                {
                    connectionRight.TileModifier.ConnectionScore = 27;
                }
                else if (currentScoreRight == 29)
                {
                    connectionRight.TileModifier.ConnectionScore = 7;
                }

                if (currentScoreDown == 20)
                {
                    connectionDown.TileModifier.ConnectionScore = 5;
                }
                else if (currentScoreDown == 24)
                {
                    connectionDown.TileModifier.ConnectionScore = 28;
                }
                else if (currentScoreDown == 30)
                {
                    connectionDown.TileModifier.ConnectionScore = 10;
                }

                if (currentScoreLeft == 17)
                {
                    connectionLeft.TileModifier.ConnectionScore = 2;
                }
                else if (currentScoreLeft == 22)
                {
                    connectionLeft.TileModifier.ConnectionScore = 29;
                }
                else if (currentScoreLeft == 27)
                {
                    connectionLeft.TileModifier.ConnectionScore = 7;
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
                    connectionRight.TileModifier.ConnectionScore = 19;
                }
                else if (currentScoreRight == 7)
                {
                    connectionRight.TileModifier.ConnectionScore = 29;
                }
                else if (currentScoreRight == 27)
                {
                    connectionRight.TileModifier.ConnectionScore = 22;
                }

                if (currentScoreDown == 5)
                {
                    connectionDown.TileModifier.ConnectionScore = 20;
                }
                else if (currentScoreDown == 10)
                {
                    connectionDown.TileModifier.ConnectionScore = 30;
                }
                else if (currentScoreDown == 28)
                {
                    connectionDown.TileModifier.ConnectionScore = 24;
                }

                if (currentScoreUp == 3)
                {
                    connectionUp.TileModifier.ConnectionScore = 18;
                }
                else if (currentScoreUp == 10)
                {
                    connectionUp.TileModifier.ConnectionScore = 28;
                }
                else if (currentScoreUp == 30)
                {
                    connectionUp.TileModifier.ConnectionScore = 24;
                }
            }
            else
            {
                thisMazeTileAttribute.ConnectionScore = 13;

                if (currentScoreRight == 19)
                {
                    connectionRight.TileModifier.ConnectionScore = 4;
                }
                else if (currentScoreRight == 22)
                {
                    connectionRight.TileModifier.ConnectionScore = 27;
                }
                else if (currentScoreRight == 29)
                {
                    connectionRight.TileModifier.ConnectionScore = 7;
                }

                if (currentScoreDown == 20)
                {
                    connectionDown.TileModifier.ConnectionScore = 5;
                }
                else if (currentScoreDown == 24)
                {
                    connectionDown.TileModifier.ConnectionScore = 28;
                }
                else if (currentScoreDown == 30)
                {
                    connectionDown.TileModifier.ConnectionScore = 10;
                }

                if (currentScoreUp == 18)
                {
                    connectionUp.TileModifier.ConnectionScore = 3;
                }
                else if (currentScoreUp == 24)
                {
                    connectionUp.TileModifier.ConnectionScore = 30;
                }
                else if (currentScoreUp == 28)
                {
                    connectionUp.TileModifier.ConnectionScore = 10;
                }
            }
        }

        else if (currentScoreThisTilePath == 33)
        {
            if (currentScoreRight == 25 || currentScoreRight == 26 || currentScoreRight == 31 || currentScoreRight == 33 || currentScoreRight == 34 ||
                currentScoreLeft == 21 || currentScoreLeft == 2 || currentScoreLeft == 31 || currentScoreLeft == 32 || currentScoreLeft == 33 ||
                currentScoreUp == 21 || currentScoreUp == 25 || currentScoreUp == 32 || currentScoreUp == 33 || currentScoreUp == 34)
            {
                if (currentScoreRight == 4)
                {
                    connectionRight.TileModifier.ConnectionScore = 19;
                }
                else if (currentScoreRight == 7)
                {
                    connectionRight.TileModifier.ConnectionScore = 29;
                }
                else if (currentScoreRight == 27)
                {
                    connectionRight.TileModifier.ConnectionScore = 22;
                }

                if (currentScoreLeft == 2)
                {
                    connectionLeft.TileModifier.ConnectionScore = 17;
                }
                else if (currentScoreLeft == 7)
                {
                    connectionLeft.TileModifier.ConnectionScore = 27;
                }
                else if (currentScoreLeft == 29)
                {
                    connectionLeft.TileModifier.ConnectionScore = 22;
                }

                if (currentScoreUp == 3)
                {
                    connectionUp.TileModifier.ConnectionScore = 18;
                }
                else if (currentScoreUp == 10)
                {
                    connectionUp.TileModifier.ConnectionScore = 28;
                }
                else if (currentScoreUp == 30)
                {
                    connectionUp.TileModifier.ConnectionScore = 24;
                }
            }
            else
            {
                thisMazeTileAttribute.ConnectionScore = 14;

                if (currentScoreRight == 19)
                {
                    connectionRight.TileModifier.ConnectionScore = 4;
                }
                else if (currentScoreRight == 22)
                {
                    connectionRight.TileModifier.ConnectionScore = 27;
                }
                else if (currentScoreRight == 29)
                {
                    connectionRight.TileModifier.ConnectionScore = 7;
                }

                if (currentScoreLeft == 17)
                {
                    connectionLeft.TileModifier.ConnectionScore = 2;
                }
                else if (currentScoreLeft == 22)
                {
                    connectionLeft.TileModifier.ConnectionScore = 29;
                }
                else if (currentScoreLeft == 27)
                {
                    connectionLeft.TileModifier.ConnectionScore = 7;
                }

                if (currentScoreUp == 18)
                {
                    connectionUp.TileModifier.ConnectionScore = 3;
                }
                else if (currentScoreUp == 24)
                {
                    connectionUp.TileModifier.ConnectionScore = 30;
                }
                else if (currentScoreUp == 28)
                {
                    connectionUp.TileModifier.ConnectionScore = 10;
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
                    connectionDown.TileModifier.ConnectionScore = 20;
                }
                else if (currentScoreDown == 10)
                {
                    connectionDown.TileModifier.ConnectionScore = 30;
                }
                else if (currentScoreDown == 28)
                {
                    connectionDown.TileModifier.ConnectionScore = 24;
                }

                if (currentScoreLeft == 2)
                {
                    connectionLeft.TileModifier.ConnectionScore = 17;
                }
                else if (currentScoreLeft == 7)
                {
                    connectionLeft.TileModifier.ConnectionScore = 27;
                }
                else if (currentScoreLeft == 29)
                {
                    connectionLeft.TileModifier.ConnectionScore = 22;
                }

                if (currentScoreUp == 3)
                {
                    connectionUp.TileModifier.ConnectionScore = 18;
                }
                else if (currentScoreUp == 10)
                {
                    connectionUp.TileModifier.ConnectionScore = 28;
                }
                else if (currentScoreUp == 30)
                {
                    connectionUp.TileModifier.ConnectionScore = 24;
                }
            }
            else
            {
                thisMazeTileAttribute.ConnectionScore = 15;

                if (currentScoreDown == 20)
                {
                    connectionDown.TileModifier.ConnectionScore = 5;
                }
                else if (currentScoreDown == 24)
                {
                    connectionDown.TileModifier.ConnectionScore = 28;
                }
                else if (currentScoreDown == 30)
                {
                    connectionDown.TileModifier.ConnectionScore = 10;
                }

                if (currentScoreLeft == 17)
                {
                    connectionLeft.TileModifier.ConnectionScore = 2;
                }
                else if (currentScoreLeft == 22)
                {
                    connectionLeft.TileModifier.ConnectionScore = 29;
                }
                else if (currentScoreLeft == 27)
                {
                    connectionLeft.TileModifier.ConnectionScore = 7;
                }

                if (currentScoreUp == 18)
                {
                    connectionUp.TileModifier.ConnectionScore = 3;
                }
                else if (currentScoreUp == 24)
                {
                    connectionUp.TileModifier.ConnectionScore = 30;
                }
                else if (currentScoreUp == 28)
                {
                    connectionUp.TileModifier.ConnectionScore = 10;
                }
            }
        }

        if (currentScoreThisTilePath != thisMazeTileAttribute.ConnectionScore)
            updatedConnections.Add(thisMazeTileAttribute);
        if (connectionRight.TileModifier && currentScoreRight != connectionRight.TileModifier.ConnectionScore)
            updatedConnections.Add(connectionRight.TileModifier);
        if (connectionDown.TileModifier && currentScoreDown != connectionDown.TileModifier.ConnectionScore)
            updatedConnections.Add(connectionDown.TileModifier);
        if (connectionLeft.TileModifier && currentScoreLeft != connectionLeft.TileModifier.ConnectionScore)
            updatedConnections.Add(connectionLeft.TileModifier);
        if (connectionUp.TileModifier && currentScoreUp != connectionUp.TileModifier.ConnectionScore)
            updatedConnections.Add(connectionUp.TileModifier);

        return updatedConnections;
    }

    private static int CalculateDoorConnectionScore(TileModifierConnectionInfo<TileObstacle> right, TileModifierConnectionInfo<TileObstacle> down, TileModifierConnectionInfo<TileObstacle> left, TileModifierConnectionInfo<TileObstacle> up)
    {
        if (right.HasConnection)
        {
            if (left.HasConnection)
            {
                return 4;
            }
            return 2;
        }
        if (left.HasConnection)
        {
            return 3;
        }
        if (down.HasConnection)
        {
            if (up.HasConnection)
            {
                return 8;
            }
            return 6;
        }
        if (up.HasConnection)
        {
            return 7;
        }
        return 1;
    }

    private static int Calculate16TileMapConnectionScore(bool right, bool down, bool left, bool up)
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
                return 9;
            }
            else if (up)
            {
                return 10;
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