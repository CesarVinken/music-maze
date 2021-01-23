using UnityEngine;

public abstract class EditorMazeTileBackgroundModifier : IEditorMazeTileBackground
{
    public virtual string Name => "";

    public Sprite Sprite => null;

    public virtual void PlaceBackground(Tile tile)
    {
        throw new System.NotImplementedException();
    }

    public virtual void PlaceBackgroundVariation(Tile tile)
    {
        throw new System.NotImplementedException();
    }

    public virtual Sprite GetSprite()
    {
        return EditorUIContainer.Instance.DefaultIcon;
    }
}