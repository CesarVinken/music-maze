using UnityEngine;

public abstract class EditorMazeTileBackgroundModifier<T> : IEditorTileBackground<T> where T : EditorMazeTile
{
    public virtual string Name => "";

    public Sprite Sprite => null;

    public virtual void PlaceBackground(T tile)
    {
        throw new System.NotImplementedException();
    }

    public virtual void PlaceBackgroundVariation(T tile)
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