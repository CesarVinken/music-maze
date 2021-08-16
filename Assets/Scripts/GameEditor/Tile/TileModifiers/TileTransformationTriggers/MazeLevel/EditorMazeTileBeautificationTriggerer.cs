using UnityEngine;

public class EditorMazeTileBeautificationTriggerer : EditorMazeTileTransformationTriggerer, IGroundMaterialModifier, IWaterMaterialModifier
{
    public override string Name { get => "Beautification Triggerer"; }

    public override Sprite Sprite { get => MazeSpriteManager.Instance.DefaultDoor[0]; } // TODO: Change

    public EditorMazeTile SelectedTile;

    public override Sprite GetSprite()
    {
        return Sprite;
    }

    public override void RemoveAllTriggerersFromTile()
    {
        if (SelectedTile)
        {
            for (int i = SelectedTile.BeautificationTriggerers.Count - 1; i >= 0; i--)
            {
                SelectedTile.BeautificationTriggerers[i].SetTileOverlayImage(TileOverlayMode.Empty);
                SelectedTile.BeautificationTriggerers.Remove(SelectedTile.BeautificationTriggerers[i]);
            }
        }
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
        // Markable tiles are triggerers, so are bridge tiles
        if (tile.Markable || tile.TryGetAttribute<PlayerSpawnpoint>() || tile.TryGetAttribute<BridgePiece>() != null)
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
            else
            {
                //
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
        GameObject.Instantiate(EditorCanvasUI.Instance.GenerateTransformationMapForTilePrefab, EditorMazeTileModificationPanel.Instance.TileModifierActionsContainer);
        GameObject.Instantiate(EditorCanvasUI.Instance.GenerateFullTileTransformationMapPrefab, EditorMazeTileModificationPanel.Instance.TileModifierActionsContainer);
    }
}
