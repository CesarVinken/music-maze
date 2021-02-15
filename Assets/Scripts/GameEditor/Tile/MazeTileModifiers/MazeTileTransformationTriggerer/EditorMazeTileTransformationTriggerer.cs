using System.Linq;
using UnityEngine;

public class EditorMazeTileTransformationTriggerer : IEditorTileTransformationTriggerer
{
    public string Name { get => "Transformation Triggerer"; }

    public Sprite Sprite => throw new System.NotImplementedException();

    public EditorMazeTile SelectedTile;

    public Sprite GetSprite()
    {
        return SpriteManager.Instance.DefaultDoor[0];
    }

    public void SetSelectedTile(EditorMazeTile tile)
    {
        if(SelectedTile != null)
        {

            SelectedTile.SetTileOverlayImage(TileOverlayMode.Empty);
            for (int i = 0; i < SelectedTile.TransformationTriggerers.Count; i++)
            {
                SelectedTile.TransformationTriggerers[i].SetTileOverlayImage(TileOverlayMode.Empty);
            }

            if (SelectedTile == tile)
            {
                SelectedTile = null;
                return;
            }
        }
        
        tile.SetTileOverlayImage(TileOverlayMode.Green);

        SelectedTile = tile;
        for (int i = 0; i < SelectedTile.TransformationTriggerers.Count; i++)
        {
            SelectedTile.TransformationTriggerers[i].SetTileOverlayImage(TileOverlayMode.Blue);
        }
    }

    public void UnsetSelectedTile()
    {
        if (SelectedTile != null)
        {
            SelectedTile.SetTileOverlayImage(TileOverlayMode.Empty);
            for (int i = 0; i < SelectedTile.TransformationTriggerers.Count; i++)
            {
                SelectedTile.TransformationTriggerers[i].SetTileOverlayImage(TileOverlayMode.Empty);
            }
        }
        SelectedTile = null;
    }

    public void HandleTransformationTriggerPlacement(EditorMazeTile tile)
    {
        if(tile.Markable || tile.TileAttributes.OfType<PlayerSpawnpoint>().Any())
        {
            // if we have a tile selected, add the markable tile as triggerer
            if(SelectedTile != null)
            {
                if (SelectedTile.TransformationTriggerers.Contains(tile))
                {
                    tile.SetTileOverlayImage(TileOverlayMode.Empty);
                    SelectedTile.TransformationTriggerers.Remove(tile);
                }
                else
                {
                    tile.SetTileOverlayImage(TileOverlayMode.Blue); // blue = triggerer
                    SelectedTile.TransformationTriggerers.Add(tile);
                }
            }
        }
        else 
        {
            SetSelectedTile(tile);
        }
    }

    public void InstantiateModifierActions()
    {
        Logger.Log("Load actions for triggerer");
        GameObject.Instantiate(EditorCanvasUI.Instance.GenerateTileTransformationMapPrefab, EditorMazeTileModificationPanel.Instance.TileModifierActionsContainer);
    }
}
