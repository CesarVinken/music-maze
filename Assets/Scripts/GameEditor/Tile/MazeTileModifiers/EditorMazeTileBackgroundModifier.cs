using UnityEngine;

public abstract class EditorMazeTileBackgroundModifier : IEditorTileBackground
{
    public virtual string Name => "";

    public Sprite Sprite => null;

    public virtual void PlaceBackground(EditorMazeTile tile)
    {
        throw new System.NotImplementedException();
    }

    public virtual void PlaceBackgroundVariation(EditorMazeTile tile)
    {
        throw new System.NotImplementedException();
    }

    public virtual Sprite GetSprite()
    {
        return EditorCanvasUI.Instance.DefaultIcon;
    }

    public void InstantiateModifierActions()
    {
    }

    public void DestroyModifierActions()
    {
    }
}