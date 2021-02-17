using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EditorMazeTileBeautificationTriggerer : EditorMazeTileTransformationTriggerer
{
    public override string Name { get => "Beautification Triggerer"; }

    public override Sprite Sprite => throw new System.NotImplementedException();

    public EditorMazeTile SelectedTile;

    public override Sprite GetSprite()
    {
        return MazeSpriteManager.Instance.DefaultDoor[0];
    }

    public void SetSelectedTile(EditorMazeTile tile)
    {
        if (SelectedTile != null)
        {

            SelectedTile.SetTileOverlayImage(TileOverlayMode.Empty);
            for (int i = 0; i < SelectedTile.BeautificationTriggerers.Count; i++)
            {
                SelectedTile.BeautificationTriggerers[i].SetTileOverlayImage(TileOverlayMode.Empty);
            }

            if (SelectedTile == tile)
            {
                SelectedTile = null;
                return;
            }
        }

        tile.SetTileOverlayImage(TileOverlayMode.Green);

        SelectedTile = tile;
        for (int i = 0; i < SelectedTile.BeautificationTriggerers.Count; i++)
        {
            SelectedTile.BeautificationTriggerers[i].SetTileOverlayImage(TileOverlayMode.Blue);
        }
    }

    public override void UnsetSelectedTile()
    {
        if (SelectedTile != null)
        {
            SelectedTile.SetTileOverlayImage(TileOverlayMode.Empty);
            for (int i = 0; i < SelectedTile.BeautificationTriggerers.Count; i++)
            {
                SelectedTile.BeautificationTriggerers[i].SetTileOverlayImage(TileOverlayMode.Empty);
            }
        }
        SelectedTile = null;
    }

    public override void HandleBeautificationTriggerPlacement(EditorMazeTile tile)
    {
        if (tile.Markable || tile.TileAttributes.OfType<PlayerSpawnpoint>().Any())
        {
            // if we have a tile selected, add the markable tile as triggerer
            if (SelectedTile != null)
            {
                if (SelectedTile.BeautificationTriggerers.Contains(tile))
                {
                    tile.SetTileOverlayImage(TileOverlayMode.Empty);
                    SelectedTile.BeautificationTriggerers.Remove(tile);
                }
                else
                {
                    tile.SetTileOverlayImage(TileOverlayMode.Blue); // blue = triggerer
                    SelectedTile.BeautificationTriggerers.Add(tile);
                }
            }
        }
        else
        {
            SetSelectedTile(tile);
        }
    }

    public override void InstantiateModifierActions()
    {
        Logger.Log("Load actions for triggerer");
        GameObject.Instantiate(EditorCanvasUI.Instance.GenerateTileTransformationMapPrefab, EditorMazeTileModificationPanel.Instance.TileModifierActionsContainer);
    }
}
