﻿using System;
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

            if (typeof(IMazeTileAttribute).IsAssignableFrom(typeof(T)))
            {
                connectedModifier = (T)neighbour.Value.MazeTileAttributes.FirstOrDefault(attribute => attribute is T);
            } 
            else
            {
                connectedModifier = (T)neighbour.Value.MazeTileBackgrounds.FirstOrDefault(background => background is T);
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
        TileConnectionVariationRegister<T> TileConnectionVariationRegister = new TileConnectionVariationRegister<T>(thisMazeTileAttribute, connectionRight, connectionDown, connectionLeft, connectionUp);
        Type modifierType = typeof(T);

        if(modifierType == typeof(MazeTilePath))
        {
            return TileConnectionVariationRegister.MazeTilePath() as List<T>;
        }
        else if(modifierType == typeof(TileObstacle))
        {
            return TileConnectionVariationRegister.TileObstacle() as List<T>;
        }
        else
        {
            Logger.Error($"Type {modifierType} was not yet implemented for Tile Connetion Register");
            return null;
        }
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