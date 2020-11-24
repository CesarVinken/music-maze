using UnityEngine;

public abstract class EditorMazeTileBackgroundModifier : IEditorMazeTileBackground
{
    public virtual string Name => "";

    public Sprite Sprite => null;

    public virtual void PlaceBackground(Tile tile)
    {
        throw new System.NotImplementedException();
    }
}