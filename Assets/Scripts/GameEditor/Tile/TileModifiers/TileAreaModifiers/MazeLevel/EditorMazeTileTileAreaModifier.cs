using UnityEngine;

public class EditorMazeTileTileAreaModifier : EditorTileAreaModifier
{
    public override string Name { get; set; }

    public override Sprite Sprite { get; set; }

    public override Sprite GetSprite()
    {
        return EditorCanvasUI.Instance.DefaultIcon;
    }

    public override void InstantiateModifierActions()
    {
        Logger.Log("Load actions for Area modifiers");
        GameObject.Instantiate(EditorCanvasUI.Instance.HandleTileAreaPrefab, EditorMazeTileModificationPanel.Instance.TileModifierActionsContainer);
    }

    public override void SetSelectedTile<T>(T tile)
    {
        SetSelectedTile(tile as EditorMazeTile);
    }

    public void SetSelectedTile(EditorMazeTile tile)
    {
        Logger.Log("Set selected tile");
        TileArea selectedTileArea = TileAreaActionHandler.Instance.SelectedTileAreaEntry?.TileArea;

        if(selectedTileArea == null)
        {
            return;
        }

        if(tile.GetTileArea(selectedTileArea) == null)
        {
            Logger.Log("set selected tile. Overlay mode blue");
            tile.SetTileOverlayImage(TileOverlayMode.Blue);
            tile.AddTileArea(selectedTileArea);
        }
        else
        {
            Logger.Log("unset selected tile. Overlay mode Empty");
            tile.SetTileOverlayImage(TileOverlayMode.Empty);
            tile.RemoveTileArea(selectedTileArea);
        }
    }
}
