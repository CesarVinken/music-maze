using UnityEngine;

public class EditorMazeTileTileAreaModifier : EditorTileAreaModifier
{
    public override string Name { get; set; }

    public override Sprite Sprite { get; set; }

    public override void DestroyModifierActions()
    {
        throw new System.NotImplementedException();
    }

    public override Sprite GetSprite()
    {
        return EditorCanvasUI.Instance.DefaultIcon;
    }

    public override void InstantiateModifierActions()
    {
        Logger.Log("Load actions for Area modifiers");
        GameObject.Instantiate(EditorCanvasUI.Instance.HandleTileAreaPrefab, EditorMazeTileModificationPanel.Instance.TileModifierActionsContainer);
    }


    public override void SetSelectedTile()
    {
        throw new System.NotImplementedException();
    }
}
