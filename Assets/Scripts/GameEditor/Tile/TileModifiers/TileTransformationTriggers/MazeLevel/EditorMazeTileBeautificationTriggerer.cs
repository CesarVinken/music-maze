using System.Linq;
using UnityEngine;

public class EditorMazeTileBeautificationTriggerer : EditorMazeTileTransformationTriggerer, IGroundMaterialModifier, IWaterMaterialModifier
{
    public override string Name { get => "Beautification Triggerer"; }

    public override Sprite Sprite { get => MazeSpriteManager.Instance.DefaultDoor[0]; }

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
        Logger.Log("HandleBeautificationTriggerPlacement");
        // Markable tiles are triggerers, so are bridge tiles
        if (tile.Markable || tile.GetAttributes().OfType<PlayerSpawnpoint>().Any() || tile.TryGetBridgePiece() != null)
        {
            Logger.Log($"we're in. tile.TryGetBridgePiece() == null? {tile.TryGetBridgePiece() == null}");
            // if we have a tile selected, add the markable tile as triggerer
            if (SelectedTile != null)
            {
                Logger.Log("SelectedTile");
                if (SelectedTile.BeautificationTriggerers.Contains(tile))
                {
                    Logger.Log("SelectedTile.BeautificationTriggerers.Contains(tile)");
                    tile.SetTileOverlayImage(TileOverlayMode.Empty);
                    SelectedTile.BeautificationTriggerers.Remove(tile);
                }
                else
                {
                    Logger.Log("else");
                    tile.SetTileOverlayImage(TileOverlayMode.Blue); // blue = triggerer
                    SelectedTile.BeautificationTriggerers.Add(tile);
                }
            }
            else
            {
                //SetSelectedTile(tile);
                Logger.Log("NO SelectedTile");
            }
        }
        else
        {
            Logger.Log("SetSelectedTile");
            SetSelectedTile(tile);
        }
    }

    public override void InstantiateModifierActions()
    {
        Logger.Log("Load actions for triggerer");
        GameObject.Instantiate(EditorCanvasUI.Instance.GenerateTileTransformationMapPrefab, EditorMazeTileModificationPanel.Instance.TileModifierActionsContainer);
    }
}
