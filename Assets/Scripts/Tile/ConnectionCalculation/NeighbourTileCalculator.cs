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

    public static TileConnectionScoreInfo MapNeighbourPlayerMarkEndsOfTile(MazeTile tile)
    {
        bool hasMarkRight = false;
        bool hasMarkDown = false;
        bool hasMarkLeft = false;
        bool hasMarkUp = false;

        foreach (KeyValuePair<ObjectDirection, Tile> item in tile.Neighbours)
        {
            MazeTile neighbour = item.Value as MazeTile;
            if (neighbour.PlayerMark == null || neighbour.PlayerMark.Owner == PlayerMarkOwner.None) continue;

            if (item.Key == ObjectDirection.Right)
            {
                hasMarkRight = true;
            }
            else if (item.Key == ObjectDirection.Down)
            {
                hasMarkDown = true;
            }
            else if (item.Key == ObjectDirection.Left)
            {
                hasMarkLeft = true;
            }
            else if (item.Key == ObjectDirection.Up)
            {
                hasMarkUp = true;
            }
        }
        return Calculate16TileMapConnectionScore(hasMarkRight, hasMarkDown, hasMarkLeft, hasMarkUp);
    }

    public static TileConnectionScoreInfo MapNeighbourPathsOfTile(Tile tile, IPathType pathType)
    {
        Logger.Log($"---------Map neighbours of {tile.GridLocation.X},{tile.GridLocation.Y}--------");
        TileModifierConnectionInfo<TilePath> pathRight = new TileModifierConnectionInfo<TilePath>(Direction.Right);
        TileModifierConnectionInfo<TilePath> pathDown = new TileModifierConnectionInfo<TilePath>(Direction.Down);
        TileModifierConnectionInfo<TilePath> pathLeft = new TileModifierConnectionInfo<TilePath>(Direction.Left);
        TileModifierConnectionInfo<TilePath> pathUp = new TileModifierConnectionInfo<TilePath>(Direction.Up);

        if (!EditorManager.InEditor)
        {
            Logger.Error("MapNeighbourPathsOfTile was not called from the editor");
            return null;
        }

        foreach (KeyValuePair<ObjectDirection, Tile> neighbour in tile.Neighbours)
        {
            Logger.Warning($"Neighbour at {neighbour.Value.GridLocation.X},{neighbour.Value.GridLocation.Y} is {neighbour.Key} of {tile.GridLocation.X},{tile.GridLocation.Y}");
            
            TilePath tilePath = neighbour.Value.TryGetTilePath();

            if (tilePath == null || tilePath.TilePathType.GetType() != pathType.GetType())
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

        return TileConnectionRegister.CalculateTileConnectionScore(pathRight, pathDown, pathLeft, pathUp);
    }

    public static TileConnectionScoreInfo MapNeighbourWaterOfTile(Tile tile, IBaseBackgroundType waterType)
    {
        Logger.Log($"---------Map neighbours of {tile.GridLocation.X},{tile.GridLocation.Y}--------");
        TileModifierConnectionInfo<TileWater> waterRight = new TileModifierConnectionInfo<TileWater>(Direction.Right);
        TileModifierConnectionInfo<TileWater> waterDown = new TileModifierConnectionInfo<TileWater>(Direction.Down);
        TileModifierConnectionInfo<TileWater> waterLeft = new TileModifierConnectionInfo<TileWater>(Direction.Left);
        TileModifierConnectionInfo<TileWater> waterUp = new TileModifierConnectionInfo<TileWater>(Direction.Up);

        if (!EditorManager.InEditor)
        {
            Logger.Error("MapNeighbourWaterOfTile was not called from the editor");
            return null;
        }

        foreach (KeyValuePair<ObjectDirection, Tile> neighbour in tile.Neighbours)
        {
            Logger.Warning($"Neighbour at {neighbour.Value.GridLocation.X},{neighbour.Value.GridLocation.Y} is {neighbour.Key} of {tile.GridLocation.X},{tile.GridLocation.Y}");

            TileWater tileWater = neighbour.Value.TryGetTileWater();

            if (tileWater == null || tileWater.TileWaterType.GetType() != waterType.GetType())
            {
                continue;
            }

            if (neighbour.Key == ObjectDirection.Right)
            {
                waterRight.HasConnection = true;
                waterRight.TileModifier = tileWater;
            }
            else if (neighbour.Key == ObjectDirection.Down)
            {
                waterDown.HasConnection = true;
                waterDown.TileModifier = tileWater;
            }
            else if (neighbour.Key == ObjectDirection.Left)
            {
                waterLeft.HasConnection = true;
                waterLeft.TileModifier = tileWater;
            }
            else if (neighbour.Key == ObjectDirection.Up)
            {
                waterUp.HasConnection = true;
                waterUp.TileModifier = tileWater;
            }
        }

        return TileConnectionRegister.CalculateTileConnectionScore(waterRight, waterDown, waterLeft, waterUp);
    }


    public static TileConnectionScoreInfo MapNeighbourObstaclesOfTile(Tile tile, ObstacleType obstacleType)
    {
        Logger.Log($"Map neighbours of {tile.GridLocation.X},{tile.GridLocation.Y}");
        TileModifierConnectionInfo<TileObstacle> obstacleRight = new TileModifierConnectionInfo<TileObstacle>(Direction.Right);
        TileModifierConnectionInfo<TileObstacle> obstacleDown = new TileModifierConnectionInfo<TileObstacle>(Direction.Down);
        TileModifierConnectionInfo<TileObstacle> obstacleLeft = new TileModifierConnectionInfo<TileObstacle>(Direction.Left);
        TileModifierConnectionInfo<TileObstacle> obstacleUp = new TileModifierConnectionInfo<TileObstacle>(Direction.Up);

        if (!EditorManager.InEditor)
        {
            Logger.Error("MapNeighbourObstaclesOfTile was not called from the editor");
            return null;
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

        return TileConnectionRegister.CalculateTileConnectionScore(obstacleRight, obstacleDown, obstacleLeft, obstacleUp);
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
            Logger.Warning($"Neighbour at {neighbour.Value.GridLocation.X},{neighbour.Value.GridLocation.Y} is {neighbour.Key} of {tile.GridLocation.X},{tile.GridLocation.Y}. We look for a {typeof(T)}");

            T connectedModifier;

            if (typeof(ITileAttribute).IsAssignableFrom(typeof(T)))
            {
                List<ITileAttribute> attributes = neighbour.Value.GetAttributes();
                connectedModifier = (T)attributes.FirstOrDefault(attribute => attribute is T);
            } 
            else
            {
                List<ITileBackground> backgrounds = neighbour.Value.GetBackgrounds();
                connectedModifier = (T)backgrounds.FirstOrDefault(background => background is T);
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

    private static List<T> CalculateTileConnectionScoreForVariations<T>(T thisMazeTileAttribute, TileModifierConnectionInfo<T> connectionRight, TileModifierConnectionInfo<T> connectionDown, TileModifierConnectionInfo<T> connectionLeft, TileModifierConnectionInfo<T> connectionUp) where T : MonoBehaviour, ITileConnectable
    {
        TileConnectionVariationRegister<T> TileConnectionVariationRegister = new TileConnectionVariationRegister<T>(thisMazeTileAttribute, connectionRight, connectionDown, connectionLeft, connectionUp);
        Type modifierType = typeof(T);

        if(modifierType == typeof(TilePath) || modifierType == typeof(TileWater))
        {
            return TileConnectionVariationRegister.MazeTileBackgroundConnection();
        }
        else if (modifierType == typeof(TileObstacle))
        {
            return TileConnectionVariationRegister.TileAttribute() as List<T>;
        }
        else
        {
            Logger.Error($"Type {modifierType} was not yet implemented for Tile Connection Register");
            return null;
        }
    }
    
    private static TileConnectionScoreInfo Calculate16TileMapConnectionScore(bool right, bool down, bool left, bool up)
    {
        if (right)
        {
            if (down)
            {
                if (left)
                {
                    if (up)
                    {
                        return new TileConnectionScoreInfo(16);
                    }
                    return new TileConnectionScoreInfo(12);
                }
                else if (up)
                {
                    return new TileConnectionScoreInfo(13);
                }
                return new TileConnectionScoreInfo(6);
            }
            else if (left)
            {
                if (up)
                {
                    return new TileConnectionScoreInfo(14);
                }
                return new TileConnectionScoreInfo(7);
            }
            else if (up)
            {
                return new TileConnectionScoreInfo(8);
            }
            return new TileConnectionScoreInfo(2);
        }
        else if (down)
        {
            if (left)
            {
                if (up)
                {
                    return new TileConnectionScoreInfo(15);
                }
                return new TileConnectionScoreInfo(9);
            }
            else if (up)
            {
                return new TileConnectionScoreInfo(10);
            }
            return new TileConnectionScoreInfo(3);
        }
        else if (left)
        {
            if (up)
            {
                return new TileConnectionScoreInfo(11);
            }
            return new TileConnectionScoreInfo(4);
        }
        else if (up)
        {
            return new TileConnectionScoreInfo(5);
        }

        return new TileConnectionScoreInfo(1);
    }
}