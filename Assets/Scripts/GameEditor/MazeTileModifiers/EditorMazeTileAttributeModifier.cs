using UnityEngine;

public abstract class EditorMazeTileAttributeModifier : IEditorMazeTileAttribute
{
    public virtual string Name => "";

    public Sprite Sprite => null;

    public virtual void PlaceAttribute(Tile tile)
    {
        throw new System.NotImplementedException();
    }
}