using System.Collections.Generic;
using UnityEngine;

public class TileCornerFillerRegister
{
    public static void TryPlaceCornerFillersForNeighbours(Tile tile)
    {
        foreach (KeyValuePair<ObjectDirection, Tile> neighbour in tile.Neighbours)
        {
            TileCornerFillerRegister.TryPlaceCornerFillers(neighbour.Value);
        }
    }

    public static void TryPlaceCornerFillers(Tile tile)
    {
        if (tile == null) return;
        Logger.Warning($"CheckForCornerFillers on location {tile.GridLocation.X}, {tile.GridLocation.Y}");
        TileBaseGround existingGround = tile.TryGetTileGround();

        if (existingGround == null)
        {
            var rightConnectionScore = tile.Neighbours[ObjectDirection.Right]?.TryGetTileGround()?.ConnectionScore;
            if (rightConnectionScore == null)
            {
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Right], TileCorner.LeftUp);
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Right], TileCorner.LeftDown);
            }
            else if (rightConnectionScore == 22)
            {
                TryAddCornerFiller(tile, TileCorner.RightUp);
                TryAddCornerFiller(tile, TileCorner.RightDown);
            }
            else if (rightConnectionScore == 31)
            {
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Right], TileCorner.LeftDown);
            }
            else if (rightConnectionScore == 34)
            {
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Right], TileCorner.LeftUp);
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Right], TileCorner.LeftDown);
            }

            var downConnectionScore = tile.Neighbours[ObjectDirection.Down]?.TryGetTileGround()?.ConnectionScore;
            if (downConnectionScore == null)
            {
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.LeftUp);
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.RightUp);
            }
            else if (downConnectionScore == 20)
            {
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.LeftUp);
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.RightUp);
            }
            else if (downConnectionScore == 23)
            {
                TryAddCornerFiller(tile, TileCorner.LeftDown);
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.LeftUp);
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.RightUp);
            }
            else if (downConnectionScore == 24)
            {
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.LeftUp);
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.RightUp);
            }
            else if (downConnectionScore == 26)
            {
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.LeftUp);
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.RightUp);
            }
            else if (downConnectionScore == 32)
            {
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.LeftUp);
            }
            else if (downConnectionScore == 33)
            {
                TryRemoveCornerFiller(tile, TileCorner.LeftDown);
                TryRemoveCornerFiller(tile, TileCorner.RightDown);
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.LeftUp);
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.RightUp);
            }
            else if (downConnectionScore == 34)
            {
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.RightUp);
            }

            var leftConnectionScore = tile.Neighbours[ObjectDirection.Left]?.TryGetTileGround()?.ConnectionScore;
            if (leftConnectionScore == null)
            {
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Left], TileCorner.RightDown);
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Left], TileCorner.RightUp);
            }
            else if (leftConnectionScore == 31)
            {
                TryRemoveCornerFiller(tile, TileCorner.LeftDown);
            }
            else if (leftConnectionScore == 32)
            {
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Left], TileCorner.RightUp);
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Left], TileCorner.LeftDown);
            }
            else if (leftConnectionScore == 33)
            {
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Left], TileCorner.RightUp);
            }

            var upConnectionScore = tile.Neighbours[ObjectDirection.Up]?.TryGetTileGround()?.ConnectionScore;
            if (upConnectionScore == null)
            {
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Up], TileCorner.LeftDown);
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Up], TileCorner.RightDown);
            }
            else if (upConnectionScore == 34)
            {
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Up], TileCorner.LeftDown);
            }
            return;
        }
        Logger.Log($"existingGround.ConnectionScore if {tile.GridLocation.X}, {tile.GridLocation.Y} is {existingGround.ConnectionScore}");

        if (existingGround.ConnectionScore == 17)
        {
            var rightConnectionScore = tile.Neighbours[ObjectDirection.Right]?.TryGetTileGround()?.ConnectionScore;
            if (rightConnectionScore == null || rightConnectionScore == 16)
            {
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Right], TileCorner.LeftUp);
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Right], TileCorner.LeftDown);
            }
            else if (rightConnectionScore == 25)
            {
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Right], TileCorner.LeftDown);
            }
            else if (rightConnectionScore == 31)
            {
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Right], TileCorner.LeftDown);
            }
            else if (rightConnectionScore == 34)
            {
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Right], TileCorner.LeftUp);
            }
        }
        else if (existingGround.ConnectionScore == 18)
        {
            var downConnectionScore = tile.Neighbours[ObjectDirection.Down]?.TryGetTileGround()?.ConnectionScore;

            if (downConnectionScore == null || downConnectionScore == 16)
            {
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.LeftUp);
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.RightUp);
            }
            else if (downConnectionScore == 20 || downConnectionScore == 23 || downConnectionScore == 26 || downConnectionScore == 24 || downConnectionScore == 30)
            {
                TryRemoveCornerFiller(tile, TileCorner.RightDown);
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.RightUp);
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.LeftUp);
                TryRemoveCornerFiller(tile, TileCorner.LeftDown);
            }
            else if (downConnectionScore == 32)
            {
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.RightUp);
            }
            else if (downConnectionScore == 33)
            {
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.RightUp);
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.LeftUp);
            }
            else if (downConnectionScore == 34)
            {
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.RightUp);
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.LeftUp);
            }
        }
        else if (existingGround.ConnectionScore == 19)
        {
            var leftConnectionScore = tile.Neighbours[ObjectDirection.Left]?.TryGetTileGround()?.ConnectionScore;
            if (leftConnectionScore == null || leftConnectionScore == 16)
            {
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Left], TileCorner.RightDown);
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Left], TileCorner.RightUp);
            }
            if (leftConnectionScore == 33)
            {
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Left], TileCorner.RightUp);
            }
        }
        else if (existingGround.ConnectionScore == 20)
        {
            var upConnectionScore = tile.Neighbours[ObjectDirection.Up]?.TryGetTileGround()?.ConnectionScore;
            if (upConnectionScore == null || upConnectionScore == 16)
            {
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Up], TileCorner.LeftDown);
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Up], TileCorner.RightDown);
            }
            else if (upConnectionScore == 18)
            {
                TryAddCornerFiller(tile, TileCorner.LeftUp);
                TryAddCornerFiller(tile, TileCorner.RightUp);
            }
            else if (upConnectionScore == 21)
            {
                TryAddCornerFiller(tile, TileCorner.LeftUp);
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Up], TileCorner.RightUp);
            }
            else if (upConnectionScore == 25)
            {
                TryAddCornerFiller(tile, TileCorner.RightUp);
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Up], TileCorner.LeftDown);
            }
            else if (upConnectionScore == 24 || upConnectionScore == 28)
            {
                TryAddCornerFiller(tile, TileCorner.RightUp);
                TryAddCornerFiller(tile, TileCorner.LeftUp);
            }
            else if (upConnectionScore == 32)
            {
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Up], TileCorner.RightDown);
            }
            else if (upConnectionScore == 31 || upConnectionScore == 34)
            {
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Up], TileCorner.LeftDown);
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Up], TileCorner.RightDown);
            }
        }
        else if (existingGround.ConnectionScore == 21)
        {
            var rightConnectionScore = tile.Neighbours[ObjectDirection.Right]?.TryGetTileGround()?.ConnectionScore;
            if (rightConnectionScore == null || rightConnectionScore == 16)
            {
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Right], TileCorner.LeftUp);
            }
            else if (rightConnectionScore == 19 || rightConnectionScore == 22 || rightConnectionScore == 29)
            {
                TryAddCornerFiller(tile, TileCorner.RightDown);
            }
            else if (rightConnectionScore == 31)
            {
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Right], TileCorner.LeftDown);
            }

            var downConnectionScore = tile.Neighbours[ObjectDirection.Down]?.TryGetTileGround()?.ConnectionScore;
            if (downConnectionScore == null)
            {
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.LeftUp);
            }
            else if (downConnectionScore == 20)
            {
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.RightUp);
            }
            else if (downConnectionScore == 23)
            {
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.LeftUp);
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.RightUp);
            }
            else if (downConnectionScore == 24)
            {
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.RightUp);
            }
            else if (downConnectionScore == 26)
            {
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.LeftUp);
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.RightUp);
            }
            else if (downConnectionScore == 32)
            {
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.RightUp);
            }
            else if (downConnectionScore == 33)
            {
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.LeftUp);
            }
        }
        else if (existingGround.ConnectionScore == 22)
        {
            var rightConnectionScore = tile.Neighbours[ObjectDirection.Right]?.TryGetTileGround()?.ConnectionScore;
            if (rightConnectionScore == null || rightConnectionScore == 16)
            {
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Right], TileCorner.LeftUp);
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Right], TileCorner.LeftDown);
            }
            else if (rightConnectionScore == 31)
            {
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Right], TileCorner.LeftDown);
            }
            else if (rightConnectionScore == 34)
            {
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Right], TileCorner.LeftDown);
            }

            var leftConnectionScore = tile.Neighbours[ObjectDirection.Left]?.TryGetTileGround()?.ConnectionScore;
            if (leftConnectionScore == null || leftConnectionScore == 16)
            {
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Left], TileCorner.RightUp);
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Left], TileCorner.RightDown);
            }
            else if (leftConnectionScore == 21)
            {
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Left], TileCorner.RightDown);
            }
            else if (leftConnectionScore == 31)
            {
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Left], TileCorner.RightDown);
            }
            else if (leftConnectionScore == 32)
            {
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Left], TileCorner.RightDown);
            }
        }
        else if (existingGround.ConnectionScore == 23)
        {
            var rightConnectionScore = tile.Neighbours[ObjectDirection.Right]?.TryGetTileGround()?.ConnectionScore;
            if (rightConnectionScore == null)
            {
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Right], TileCorner.LeftDown);
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Right], TileCorner.LeftUp);
            }
            else if (rightConnectionScore == 19)
            {
                TryAddCornerFiller(tile, TileCorner.RightUp);
            }
            else if (rightConnectionScore == 26)
            {
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Right], TileCorner.LeftUp);
            }
            else if (rightConnectionScore == 31)
            {
                TryAddCornerFiller(tile, TileCorner.RightUp);
                TryRemoveCornerFiller(tile, TileCorner.RightUp);
            }
            else if (rightConnectionScore == 33)
            {
                TryRemoveCornerFiller(tile, TileCorner.LeftUp);
            }

            var upConnectionScore = tile.Neighbours[ObjectDirection.Up]?.TryGetTileGround()?.ConnectionScore;
            if (upConnectionScore == null)
            {
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Up], TileCorner.LeftDown);
            }
            else if (upConnectionScore == 18 || upConnectionScore == 24 || upConnectionScore == 28)
            {
                TryAddCornerFiller(tile, TileCorner.RightUp);
                TryRemoveCornerFiller(tile, TileCorner.RightDown);
                TryRemoveCornerFiller(tile, TileCorner.LeftDown);
                TryAddCornerFiller(tile, TileCorner.LeftUp);
            }
            else if (upConnectionScore == 21)
            {
                TryAddCornerFiller(tile, TileCorner.LeftUp);
            }
            else if (upConnectionScore == 31)
            {
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Up], TileCorner.RightDown);
            }
            else if (upConnectionScore == 32)
            {
                TryAddCornerFiller(tile, TileCorner.LeftUp);
            }
            else if (upConnectionScore == 32)
            {
                TryAddCornerFiller(tile, TileCorner.LeftUp);
            }
            else if (upConnectionScore == 33)
            {
                TryRemoveCornerFiller(tile, TileCorner.RightUp);
            }
            else if (upConnectionScore == 34)
            {
                TryAddCornerFiller(tile, TileCorner.RightUp);
            }
        }
        else if (existingGround.ConnectionScore == 24)
        {
            var downConnectionScore = tile.Neighbours[ObjectDirection.Down]?.TryGetTileGround()?.ConnectionScore;
            if (downConnectionScore == null || downConnectionScore == 16)
            {
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.LeftUp);
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.RightUp);
            }
            else if (downConnectionScore == 23)
            {
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.RightUp);
            }
            else if (downConnectionScore == 26)
            {
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.LeftUp);
            }
            else if (downConnectionScore == 32)
            {
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.RightUp);
            }
            else if (downConnectionScore == 33)
            {
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.LeftUp);
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.RightUp);
            }
            else if (downConnectionScore == 34)
            {
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.LeftUp);
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.RightUp);
            }

            var upConnectionScore = tile.Neighbours[ObjectDirection.Up]?.TryGetTileGround()?.ConnectionScore;
            if (upConnectionScore == 18)
            {
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Up], TileCorner.LeftUp);
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Up], TileCorner.RightUp);
            }
            else if (upConnectionScore == 21)
            {
                TryRemoveCornerFiller(tile, TileCorner.RightUp);
                TryAddCornerFiller(tile, TileCorner.LeftUp);
            }
            else if (upConnectionScore == 24)
            {
                TryAddCornerFiller(tile, TileCorner.LeftUp);
                TryAddCornerFiller(tile, TileCorner.RightUp);
            }
            else if (upConnectionScore == 25)
            {
                TryAddCornerFiller(tile, TileCorner.RightUp);
                TryRemoveCornerFiller(tile, TileCorner.LeftUp);
            }
            else if (upConnectionScore == 34)
            {
                TryAddCornerFiller(tile, TileCorner.RightUp);
            }
        }
        else if (existingGround.ConnectionScore == 25)
        {
            var downConnectionScore = tile.Neighbours[ObjectDirection.Down]?.TryGetTileGround()?.ConnectionScore;
            if (downConnectionScore == null)
            {
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.LeftUp);
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.RightUp);
            }
            else if (downConnectionScore == 18)
            {
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.LeftUp);
                TryAddCornerFiller(tile, TileCorner.LeftDown);
            }
            else if (downConnectionScore == 20)
            {
                TryAddCornerFiller(tile, TileCorner.LeftDown);
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.LeftUp);
            }
            else if (downConnectionScore == 23)
            {
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.RightUp);
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.LeftUp);
                TryAddCornerFiller(tile, TileCorner.LeftDown);
            }
            else if (downConnectionScore == 24)
            {
                TryAddCornerFiller(tile, TileCorner.LeftDown);
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.RightUp);
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.LeftUp);
            }
            else if (downConnectionScore == 26)
            {
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.RightUp);
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.LeftUp);
            }
            else if (downConnectionScore == 32)
            {
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.LeftUp);
                TryAddCornerFiller(tile, TileCorner.LeftDown);
            }
            else if (downConnectionScore == 32)
            {
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.LeftUp);
            }
            else if (downConnectionScore == 34)
            {
                TryRemoveCornerFiller(tile, TileCorner.LeftDown);
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.LeftUp);
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.RightUp);
            }

            var leftConnectionScore = tile.Neighbours[ObjectDirection.Left]?.TryGetTileGround()?.ConnectionScore;
            if (leftConnectionScore == null)
            {
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Left], TileCorner.RightUp);
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Left], TileCorner.RightDown);
            }
            else if (leftConnectionScore == 21)
            {
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Left], TileCorner.RightDown);
            }
            else if (leftConnectionScore == 22)
            {
                TryAddCornerFiller(tile, TileCorner.LeftDown);
            }
            else if (leftConnectionScore == 31)
            {
                TryRemoveCornerFiller(tile, TileCorner.LeftDown);
            }
        }
        else if (existingGround.ConnectionScore == 26)
        {
            var leftConnectionScore = tile.Neighbours[ObjectDirection.Left]?.TryGetTileGround()?.ConnectionScore;
            if (leftConnectionScore == 23)
            {
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Left], TileCorner.RightUp);
            }
            else if (leftConnectionScore == 17 || leftConnectionScore == 21 || leftConnectionScore == 22 || leftConnectionScore == 27)
            {
                var upConnectionScore = tile.Neighbours[ObjectDirection.Up]?.TryGetTileGround()?.ConnectionScore;
                if (upConnectionScore == 18 || upConnectionScore == 24 || upConnectionScore == 28)
                {
                    TryAddCornerFiller(tile, TileCorner.RightUp);
                    TryRemoveCornerFiller(tile, TileCorner.RightDown);
                    TryRemoveCornerFiller(tile, TileCorner.LeftDown);
                    TryAddCornerFiller(tile, TileCorner.LeftUp);
                }
                else if (upConnectionScore == 21)
                {
                    TryAddCornerFiller(tile, TileCorner.LeftUp);
                    TryRemoveCornerFiller(tile, TileCorner.RightUp);
                }
                else if (upConnectionScore == 22)
                {
                    TryAddCornerFiller(tile, TileCorner.LeftUp);
                }
                else if (upConnectionScore == 25)
                {
                    TryAddCornerFiller(tile, TileCorner.RightUp);
                    TryRemoveCornerFiller(tile, TileCorner.LeftUp);
                    TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Up], TileCorner.LeftDown);
                }
                else if (upConnectionScore == 32)
                {
                    TryAddCornerFiller(tile, TileCorner.LeftUp);
                }
            }

            var uppConnectionScore = tile.Neighbours[ObjectDirection.Up]?.TryGetTileGround()?.ConnectionScore;
            if (uppConnectionScore == 31)
            {
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Up], TileCorner.LeftDown);
            }
        }
        else if (existingGround.ConnectionScore == 31)
        {
            var rightConnectionScore = tile.Neighbours[ObjectDirection.Right]?.TryGetTileGround()?.ConnectionScore;
            if (rightConnectionScore == null || rightConnectionScore == 16)
            {
                TryAddCornerFiller(tile, TileCorner.LeftUp);
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Right], TileCorner.LeftDown);
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Right], TileCorner.LeftUp);
            }
            else if (rightConnectionScore == 25)
            {
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Right], TileCorner.LeftDown);
            }
            else if (rightConnectionScore == 31)
            {
                TryAddCornerFiller(tile, TileCorner.LeftUp);
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Right], TileCorner.LeftDown);
            }
            else if (rightConnectionScore == 34)
            {
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Right], TileCorner.LeftUp);
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Right], TileCorner.LeftDown);
            }

            var downConnectionScore = tile.Neighbours[ObjectDirection.Down]?.TryGetTileGround()?.ConnectionScore;
            if (downConnectionScore == null || downConnectionScore == 16)
            {
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.RightUp);
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.LeftUp);
            }
            else if (downConnectionScore == 20)
            {
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.RightUp);
            }
            else if (downConnectionScore == 23)
            {
                TryAddCornerFiller(tile, TileCorner.LeftDown);
                TryRemoveCornerFiller(tile, TileCorner.RightDown);
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.LeftUp);
            }
            else if (downConnectionScore == 24)
            {
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.RightUp);
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.LeftUp);
            }
            else if (downConnectionScore == 26)
            {
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.RightUp);
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.LeftUp);
            }
            else if (downConnectionScore == 32)
            {
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.RightUp);
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.LeftUp);
            }
            else if (downConnectionScore == 33)
            {
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.LeftUp);
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.RightUp);
            }
            else if (downConnectionScore == 34)
            {
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.RightUp);
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.LeftUp);
            }

            var leftConnectionScore = tile.Neighbours[ObjectDirection.Left]?.TryGetTileGround()?.ConnectionScore;
            if (leftConnectionScore == null || leftConnectionScore == 16)
            {
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Left], TileCorner.RightUp);
                TryRemoveCornerFiller(tile, TileCorner.LeftUp);
                TryRemoveCornerFiller(tile, TileCorner.LeftDown);
            }
            else if (leftConnectionScore == 21)
            {
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Left], TileCorner.RightDown);
                TryRemoveCornerFiller(tile, TileCorner.LeftDown);
            }
            else if (leftConnectionScore == 23)
            {
                TryAddCornerFiller(tile, TileCorner.LeftDown);
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Left], TileCorner.RightDown);
            }
            else if (leftConnectionScore == 31)
            {
                TryRemoveCornerFiller(tile, TileCorner.LeftDown);
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Left], TileCorner.RightDown);
            }
            else if (leftConnectionScore == 31)
            {
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Left], TileCorner.RightDown);
                TryRemoveCornerFiller(tile, TileCorner.LeftDown);
            }
        }
        else if (existingGround.ConnectionScore == 32)
        {
            var rightConnectionScore = tile.Neighbours[ObjectDirection.Right]?.TryGetTileGround()?.ConnectionScore;
            if (rightConnectionScore == null)
            {
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Right], TileCorner.LeftDown);
            }
            else if (rightConnectionScore == 25)
            {
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Right], TileCorner.LeftDown);
                TryRemoveCornerFiller(tile, TileCorner.RightDown);
                TryAddCornerFiller(tile, TileCorner.RightUp);
            }
            else if (rightConnectionScore == 31)
            {
                TryRemoveCornerFiller(tile, TileCorner.LeftUp);
            }
            else if (rightConnectionScore == 34)
            {
                TryRemoveCornerFiller(tile, TileCorner.LeftUp);
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Right], TileCorner.LeftUp);
            }

            var downConnectionScore = tile.Neighbours[ObjectDirection.Down]?.TryGetTileGround()?.ConnectionScore;
            if (downConnectionScore == null || downConnectionScore == 16)
            {
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.LeftUp);
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.RightUp);
            }
            else if (downConnectionScore == 20)
            {
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.LeftUp);
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.RightUp);
            }
            else if (downConnectionScore == 23)
            {
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.RightUp);
            }
            else if (downConnectionScore == 24)
            {
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.RightUp);
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.LeftUp);
            }
            else if (downConnectionScore == 26)
            {
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.RightUp);
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.LeftUp);
            }
            else if (downConnectionScore == 32)
            {
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.RightUp);
            }
            else if (downConnectionScore == 33)
            {
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.RightUp);
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.LeftUp);
            }
            else if (downConnectionScore == 34)
            {
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.RightUp);
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.LeftUp);
            }

            var upConnectionScore = tile.Neighbours[ObjectDirection.Up]?.TryGetTileGround()?.ConnectionScore;
            if (upConnectionScore == null)
            {
                TryRemoveCornerFiller(tile, TileCorner.LeftUp);
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Up], TileCorner.LeftDown);
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Up], TileCorner.RightDown);
            }
            else if (upConnectionScore == 25)
            {
                TryRemoveCornerFiller(tile, TileCorner.LeftUp);
                TryAddCornerFiller(tile, TileCorner.RightUp);
            }
            else if (upConnectionScore == 21 || upConnectionScore == 32)
            {
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Up], TileCorner.RightDown);
                TryRemoveCornerFiller(tile, TileCorner.RightUp);
            }
            else if (upConnectionScore == 31)
            {
                TryRemoveCornerFiller(tile, TileCorner.LeftUp);
                TryRemoveCornerFiller(tile, TileCorner.RightUp);
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Up], TileCorner.LeftDown);
            }
            else if (upConnectionScore == 34)
            {
                TryAddCornerFiller(tile, TileCorner.RightUp);
                TryRemoveCornerFiller(tile, TileCorner.LeftUp);
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Up], TileCorner.LeftDown);
            }
        }
        else if (existingGround.ConnectionScore == 33)
        {
            var rightConnectionScore = tile.Neighbours[ObjectDirection.Right]?.TryGetTileGround()?.ConnectionScore;
            if (rightConnectionScore == null)
            {
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Right], TileCorner.LeftDown);
            }
            else if (rightConnectionScore == 26)
            {
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Right], TileCorner.LeftUp);
            }
            else if (rightConnectionScore == 31)
            {
                TryAddCornerFiller(tile, TileCorner.RightUp);
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Right], TileCorner.LeftDown);
            }
            else if (rightConnectionScore == 33)
            {
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Right], TileCorner.LeftUp);
            }
            else if (rightConnectionScore == 34)
            {
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Right], TileCorner.LeftDown);
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Right], TileCorner.RightUp);
            }

            var leftConnectionScore = tile.Neighbours[ObjectDirection.Left]?.TryGetTileGround()?.ConnectionScore;
            if (leftConnectionScore == null)
            {
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Left], TileCorner.RightUp);
            }
            else if (leftConnectionScore == 23)
            {
                TryRemoveCornerFiller(tile, TileCorner.LeftUp);
            }
            else if (leftConnectionScore == 32)
            {
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Left], TileCorner.RightUp);
            }
            else if (leftConnectionScore == 33)
            {
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Left], TileCorner.RightUp);
            }

            var upConnectionScore = tile.Neighbours[ObjectDirection.Up]?.TryGetTileGround()?.ConnectionScore;
            if (upConnectionScore == 21)
            {
                TryAddCornerFiller(tile, TileCorner.LeftUp);
            }
            else if (upConnectionScore == 21)
            {
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Up], TileCorner.LeftDown);
            }
            else if (upConnectionScore == 25)
            {
                TryAddCornerFiller(tile, TileCorner.RightUp);
            }
            else if (upConnectionScore == 31)
            {
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Up], TileCorner.LeftDown);
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Up], TileCorner.RightDown);
            }
            else if (upConnectionScore == 32)
            {
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Up], TileCorner.RightDown);
            }
        }
        else if (existingGround.ConnectionScore == 34)
        {
            var downConnectionScore = tile.Neighbours[ObjectDirection.Down]?.TryGetTileGround()?.ConnectionScore;
            if (downConnectionScore == null)
            {
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.RightUp);
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.LeftUp);
            }
            else if (downConnectionScore == 20)
            {
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.LeftUp);
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.RightUp);
            }
            else if (downConnectionScore == 23)
            {
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.LeftUp);
            }
            else if (downConnectionScore == 24)
            {
                TryAddCornerFiller(tile, TileCorner.LeftDown);
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.LeftUp);
            }
            else if (downConnectionScore == 26)
            {
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.RightUp);
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.LeftUp);
            }
            else if (downConnectionScore == 33)
            {
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.RightUp);
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.LeftUp);
            }
            else if (downConnectionScore == 34)
            {
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.LeftUp);
            }

            var leftConnectionScore = tile.Neighbours[ObjectDirection.Left]?.TryGetTileGround()?.ConnectionScore;
            if (leftConnectionScore == null || leftConnectionScore == 16)
            {
                TryRemoveCornerFiller(tile, TileCorner.LeftUp);
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Left], TileCorner.RightUp);
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Left], TileCorner.RightDown);
            }
            else if (leftConnectionScore == 23)
            {
                TryRemoveCornerFiller(tile, TileCorner.LeftUp);
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Left], TileCorner.RightUp);
            }
            else if (leftConnectionScore == 32)
            {
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Left], TileCorner.RightUp);
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Left], TileCorner.RightDown);
            }

            var upConnectionScore = tile.Neighbours[ObjectDirection.Up]?.TryGetTileGround()?.ConnectionScore;
            if (upConnectionScore == null)
            {
                TryRemoveCornerFiller(tile, TileCorner.RightUp);
            }
            else if (upConnectionScore == 25)
            {
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Up], TileCorner.LeftDown);
            }
        }
    }

    public static void TryAddCornerFiller(Tile tile, TileCorner tileCorner)
    {
        if (tile == null)
        {
            return;
        }

        //Check if there is already a tilecorner
        if (tile.TryGetCornerFiller(tileCorner))
        {
            return;
        }
        Logger.Warning($"TryAddCornerFiller on {tile.GridLocation.X}, {tile.GridLocation.Y} in the corner {tileCorner}");

        //create cornerfiller
        GameObject backgroundGO = GameObject.Instantiate(OverworldManager.Instance.GetTileBackgroundPrefab<TileCornerFiller>(), tile.BackgroundsContainer);
        TileCornerFiller cornerFiller = backgroundGO.GetComponent<TileCornerFiller>();

        cornerFiller.SetTile(tile);
        cornerFiller.WithType(new OverworldDefaultGroundType());
        cornerFiller.WithCorner(tileCorner); // pick sprite based on corner

        tile.AddCornerFiller(cornerFiller);
    }

    public static void TryRemoveCornerFiller(Tile tile, TileCorner tileCorner)
    {
        if (tile == null) return;

        tile.TryRemoveCornerFiller(tileCorner);
    }
}
