using UnityEngine;

public abstract class EditorMazeTileBackgroundModifier : IEditorMazeTileBackground
{
    public virtual string Name => "";

    public Sprite Sprite => null;

    public virtual void PlaceBackground(EditorTile tile)
    {
        throw new System.NotImplementedException();
    }

    public virtual void PlaceBackgroundVariation(EditorTile tile)
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