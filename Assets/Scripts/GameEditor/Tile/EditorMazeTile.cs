using System.Collections.Generic;
using UnityEngine;

public class EditorMazeTile : MazeTile
{
    [Header("Editor")]

    [SerializeField] private SpriteRenderer _overlaySpriteRenderer;

    public List<EditorMazeTile> TransformationTriggerers = new List<EditorMazeTile>(); // used in the editor for non-markable tiles and lists their triggerer.

    public void AddNeighbours(EditorMazeLevel level)
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

    public void RemoveTileAsTransformationTrigger()
    {
        for (int i = 0; i < MazeLevelManager.Instance.EditorLevel.Tiles.Count; i++)
        {
            EditorMazeTile tile = MazeLevelManager.Instance.EditorLevel.Tiles[i];
            if (tile.TransformationTriggerers.Contains(this))
            {
                tile.TransformationTriggerers.Remove(this);
            }
        }
    }

    public void RemoveTransformationTriggerers()
    {
        TransformationTriggerers.Clear();
    }

    public void SetTileOverlayImage(TileOverlayMode tileOverlayMode)
    {
        switch (tileOverlayMode)
        {
            case TileOverlayMode.Empty:
                _overlaySpriteRenderer.color = new Color(0, 0, 0, 0);
                break;
            case TileOverlayMode.Blue:
                _overlaySpriteRenderer.color = new Color(0, 0, 1, 0.5f);
                break;
            case TileOverlayMode.Green:
                _overlaySpriteRenderer.color = new Color(0, 1, 0, 0.5f);
                break;
            case TileOverlayMode.Yellow:
                _overlaySpriteRenderer.color = new Color(1, 1, 0, 0.5f);
                break;
            default:
                Logger.Error($"Tile overlay mode {tileOverlayMode} was not yet implemented");
                break;
        }
    }
}
