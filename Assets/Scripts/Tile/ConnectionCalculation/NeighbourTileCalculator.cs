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

        foreach (KeyValuePair<Direction, Tile> item in tile.Neighbours)
        {
            MazeTile neighbour = item.Value as MazeTile;
            if (neighbour == null || neighbour.PlayerMark == null || neighbour.PlayerMark.Owner == PlayerMarkOwner.None) continue;

            if (item.Key == Direction.Right)
            {
                hasMarkRight = true;
            }
            else if (item.Key == Direction.Down)
            {
                hasMarkDown = true;
            }
            else if (item.Key == Direction.Left)
            {
                hasMarkLeft = true;
            }
            else if (item.Key == Direction.Up)
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

        foreach (KeyValuePair<Direction, Tile> neighbour in tile.Neighbours)
        {
            if (!neighbour.Value)
            {
                // if there is no tile as neighbour, it must mean it as the level edge. This counts as a connection.
                if (neighbour.Key == Direction.Right)
                {
                    pathRight.HasConnection = true;
                }
                else if (neighbour.Key == Direction.Down)
                {
                    pathDown.HasConnection = true;
                }
                else if (neighbour.Key == Direction.Left)
                {
                    pathLeft.HasConnection = true;
                }
                else if (neighbour.Key == Direction.Up)
                {
                    pathUp.HasConnection = true;
                }
                continue;
            }

            Logger.Warning($"Neighbour at {neighbour.Value.GridLocation.X},{neighbour.Value.GridLocation.Y} is {neighbour.Key} of {tile.GridLocation.X},{tile.GridLocation.Y}.");
            
            // Check if the neighbour has a connection which is a PATH or a BRIDGE
            TilePath tilePath = neighbour.Value.TryGetTilePath();
            if (tilePath == null || tilePath.TilePathType?.GetType() != pathType.GetType())
            {
                BridgePiece bridgePiece = neighbour.Value.TryGetAttribute<BridgePiece>();
                if (bridgePiece == null)
                {
                    continue;
                }

                bool hasBridgeConnection = false;

                if ((neighbour.Key == Direction.Right || neighbour.Key == Direction.Left) &&
                    bridgePiece.BridgePieceDirection == BridgePieceDirection.Horizontal)
                {
                    hasBridgeConnection = true;
                }
                else if ((neighbour.Key == Direction.Down || neighbour.Key == Direction.Up) &&
                    bridgePiece.BridgePieceDirection == BridgePieceDirection.Vertical)
                {
                    hasBridgeConnection = true;
                }

                if (hasBridgeConnection == false)
                {
                    continue;
                }
            }

            if (neighbour.Key == Direction.Right)
            {
                pathRight.HasConnection = true;
                pathRight.TileModifier = tilePath;
            }
            else if (neighbour.Key == Direction.Down)
            {
                pathDown.HasConnection = true;
                pathDown.TileModifier = tilePath;
            }
            else if (neighbour.Key == Direction.Left)
            {
                pathLeft.HasConnection = true;
                pathLeft.TileModifier = tilePath;
            }
            else if (neighbour.Key == Direction.Up)
            {
                pathUp.HasConnection = true;
                pathUp.TileModifier = tilePath;
            }
        }

        return TileConnectionRegister.CalculateTileConnectionScore(pathRight, pathDown, pathLeft, pathUp);
    }

    public static TileConnectionScoreInfo MapGroundConnectionsWithNeighbours(Tile tile, IBaseBackgroundType groundType)
    {
        Logger.Log($"---------Map neighbours of {tile.GridLocation.X},{tile.GridLocation.Y}--------");
        TileModifierConnectionInfo<TileBaseGround> groundRight = new TileModifierConnectionInfo<TileBaseGround>(Direction.Right);
        TileModifierConnectionInfo<TileBaseGround> groundDown = new TileModifierConnectionInfo<TileBaseGround>(Direction.Down);
        TileModifierConnectionInfo<TileBaseGround> groundLeft = new TileModifierConnectionInfo<TileBaseGround>(Direction.Left);
        TileModifierConnectionInfo<TileBaseGround> groundUp = new TileModifierConnectionInfo<TileBaseGround>(Direction.Up);

        if (!EditorManager.InEditor)
        {
            Logger.Error("MapNeighbourGroundOfTile was not called from the editor");
            return null;
        }

        foreach (KeyValuePair<Direction, Tile> neighbour in tile.Neighbours)
        {
            if (!neighbour.Value)
            {
                // if there is no tile as neighbour, it must mean it as the level edge. This counts as a connection.
                // But only if the main tile itself is a land tile
                if(tile.TileMainMaterial.GetType() == typeof(GroundMainMaterial))
                {
                    if (neighbour.Key == Direction.Right)
                    {
                        groundRight.HasConnection = true;
                    }
                    else if (neighbour.Key == Direction.Down)
                    {
                        groundDown.HasConnection = true;
                    }
                    else if (neighbour.Key == Direction.Left)
                    {
                        groundLeft.HasConnection = true;
                    }
                    else if (neighbour.Key == Direction.Up)
                    {
                        groundUp.HasConnection = true;
                    }
                }
                continue;
            }
            
            Logger.Warning($"Neighbour at {neighbour.Value.GridLocation.X},{neighbour.Value.GridLocation.Y} is {neighbour.Key} of {tile.GridLocation.X},{tile.GridLocation.Y}. Its main material is {neighbour.Value.TileMainMaterial.GetType()}");

            TileBaseGround tileGround = neighbour.Value.TryGetTileGround();
            //if (tileGround == null || tileGround.TileGroundType.GetType() != groundType.GetType())
            //{
            //    continue;
            //}

            if (neighbour.Value.TileMainMaterial?.GetType() != typeof(GroundMainMaterial))
            {
                continue;
            }

            if (neighbour.Key == Direction.Right)
            {
                groundRight.HasConnection = true;
                groundRight.TileModifier = tileGround;
            }
            else if (neighbour.Key == Direction.Down)
            {
                groundDown.HasConnection = true;
                groundDown.TileModifier = tileGround;
            }
            else if (neighbour.Key == Direction.Left)
            {
                groundLeft.HasConnection = true;
                groundLeft.TileModifier = tileGround;
            }
            else if (neighbour.Key == Direction.Up)
            {
                groundUp.HasConnection = true;
                groundUp.TileModifier = tileGround;
            }
        }

        return TileConnectionRegister.CalculateInversedTileConnectionScore(groundRight, groundDown, groundLeft, groundUp);
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

        foreach (KeyValuePair<Direction, Tile> neighbour in tile.Neighbours)
        {
            if (!neighbour.Value) continue;
            
            Logger.Warning($"Neighbour at {neighbour.Value.GridLocation.X},{neighbour.Value.GridLocation.Y} is {neighbour.Key} of {tile.GridLocation.X},{tile.GridLocation.Y}");
            
            TileObstacle tileObstacle = neighbour.Value.TryGetTileObstacle();

            if (tileObstacle == null || tileObstacle.ObstacleType != obstacleType)
            {
                continue;
            }

            if (neighbour.Key == Direction.Right)
            {
                obstacleRight.HasConnection = true;
                obstacleRight.TileModifier = tileObstacle;
            }
            else if (neighbour.Key == Direction.Down)
            {
                obstacleDown.HasConnection = true;
                obstacleDown.TileModifier = tileObstacle;
            }
            else if (neighbour.Key == Direction.Left)
            {
                obstacleLeft.HasConnection = true;
                obstacleLeft.TileModifier = tileObstacle;
            }
            else if (neighbour.Key == Direction.Up)
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

        foreach (KeyValuePair<Direction, Tile> neighbour in tile.Neighbours)
        {
            if (!neighbour.Value) continue;

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

            if (neighbour.Key == Direction.Right)
            {
                connectionRight.HasConnection = true;
                connectionRight.TileModifier = connectedModifier;
            }
            else if (neighbour.Key == Direction.Down)
            {     
                connectionDown.HasConnection = true;
                connectionDown.TileModifier = connectedModifier;
            }
            else if (neighbour.Key == Direction.Left)
            {
                connectionLeft.HasConnection = true;
                connectionLeft.TileModifier = connectedModifier;
            }
            else if (neighbour.Key == Direction.Up)
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

        if(modifierType == typeof(TilePath) || modifierType == typeof(TileBaseGround))
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