using System.Collections.Generic;
using UnityEngine;

public class InGameMazeTile : MazeTile
{
    [SerializeField] private List<InGameMazeTile> _tilesToTransform = new List<InGameMazeTile>();

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (!Walkable) return;

        PlayerCharacter player = collision.gameObject.GetComponent<PlayerCharacter>();
        if (player != null)
        {
            //Logger.Log("{0} entered tile {1},{2}", player.name, GridLocation.X, GridLocation.Y);
            if (GameRules.GamePlayerType == GamePlayerType.NetworkMultiplayer && !player.PhotonView.IsMine) return;

            player.UpdateCurrentGridLocation(GridLocation);

            if (PlayerMarkRenderer.sprite != null) return;

            if (!Markable) return;

            MazeLevelManager.Instance.SetTileMarker(this, player);
        }
    }

    public void AddTilesToTransform(List<InGameMazeTile> tilesToTransform)
    {
        _tilesToTransform = tilesToTransform;
    }

    //public void AddNeighbours(InGameMazeLevel level)
    //{
    //    //Add Right
    //    if (GridLocation.X < level.LevelBounds.X)
    //    {
    //        Neighbours.Add(ObjectDirection.Right, level.TilesByLocation[new GridLocation(GridLocation.X + 1, GridLocation.Y)]);
    //    }

    //    //Add Down
    //    if (GridLocation.Y > 0)
    //    {
    //        Neighbours.Add(ObjectDirection.Down, level.TilesByLocation[new GridLocation(GridLocation.X, GridLocation.Y - 1)]);
    //    }

    //    //Add Left
    //    if (GridLocation.X > 0)
    //    {
    //        Neighbours.Add(ObjectDirection.Left, level.TilesByLocation[new GridLocation(GridLocation.X - 1, GridLocation.Y)]);
    //    }

    //    //Add Up
    //    if (GridLocation.Y < level.LevelBounds.Y)
    //    {
    //        Neighbours.Add(ObjectDirection.Up, level.TilesByLocation[new GridLocation(GridLocation.X, GridLocation.Y + 1)]);
    //    }
    //}

    // Once the tile is marked, trigger the transformation of all tiles set up for this tile in the TilesToTransform list
    public void TriggerTransformations()
    {
        if (TransformationState == TransformationState.Bleak)
            TriggerTransformationOnSelf();

        for (int i = 0; i < _tilesToTransform.Count; i++)
        {
            InGameMazeTile tileToTransform = _tilesToTransform[i];

            if (tileToTransform.TransformationState == TransformationState.Colourful)
                continue;

            tileToTransform.TriggerTransformationOnSelf();
        }
    }

    public void TriggerTransformationOnSelf()
    {
        for (int i = 0; i < _tileAttributes.Count; i++)
        {
            ITransformable attribute = _tileAttributes[i] as ITransformable;
            if (attribute != null)
            {
                attribute.TriggerTransformation();
            }
        }

        for (int j = 0; j < _tileBackgrounds.Count; j++)
        {
            ITransformable background = _tileBackgrounds[j] as ITransformable;
            if (background != null)
            {
                background.TriggerTransformation();
            }
        }

        TransformationState = TransformationState.Colourful;
    }
}