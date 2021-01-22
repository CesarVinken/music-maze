using System.Collections.Generic;
using UnityEngine;

public class InGameTile : Tile
{
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (!Walkable) return;

        PlayerCharacter player = collision.gameObject.GetComponent<PlayerCharacter>();
        if (player != null)
        {
            //Logger.Log("{0} entered tile {1},{2}", player.name, GridLocation.X, GridLocation.Y);
            if (GameManager.Instance.GameType == GameType.Multiplayer && !player.PhotonView.IsMine) return;

            player.UpdateCurrentGridLocation(GridLocation);

            if (PlayerMarkRenderer.sprite != null) return;

            if (!Markable) return;

            MazeLevelManager.Instance.SetTileMarker(this, player);
        }
    }

    public void AddNeighbours(InGameMazeLevel level)
    {
        //Add Right
        if (GridLocation.X < level.LevelBounds.X)
        {
            Neighbours.Add(ObjectDirection.Right, level.TilesByLocation[new GridLocation(GridLocation.X + 1, GridLocation.Y)]);
        }

        //Add Down
        if (GridLocation.Y > 0)
        {
            Neighbours.Add(ObjectDirection.Down, level.TilesByLocation[new GridLocation(GridLocation.X, GridLocation.Y - 1)]);
        }

        //Add Left
        if (GridLocation.X > 0)
        {
            Neighbours.Add(ObjectDirection.Left, level.TilesByLocation[new GridLocation(GridLocation.X - 1, GridLocation.Y)]);
        }

        //Add Up
        if (GridLocation.Y < level.LevelBounds.Y)
        {
            Neighbours.Add(ObjectDirection.Up, level.TilesByLocation[new GridLocation(GridLocation.X, GridLocation.Y + 1)]);
        }
    }

    // Once the tile is marked, trigger the transformation of all tiles set up for this tile in the TilesToTransform list
    public void TriggerTransformations()
    {
        if (TransformationState == TransformationState.Bleak)
            TriggerTransformationOnSelf();

        for (int i = 0; i < RegisteredTilesToTransform.Count; i++)
        {
            InGameTile tileToTransform = RegisteredTilesToTransform[i] as InGameTile;

            if (TransformationState == TransformationState.Bleak)
                continue;

            tileToTransform.TriggerTransformationOnSelf();
        }
    }

    public void TriggerTransformationOnSelf()
    {
        for (int i = 0; i < MazeTileAttributes.Count; i++)
        {
            ITransformable attribute = MazeTileAttributes[i] as ITransformable;
            if (attribute != null)
            {
                attribute.TriggerTransformation();
            }
        }

        for (int j = 0; j < MazeTileBackgrounds.Count; j++)
        {
            ITransformable background = MazeTileBackgrounds[j] as ITransformable;
            if (background != null)
            {
                background.TriggerTransformation();
            }
        }

        TransformationState = TransformationState.Colourful;
    }
}