using UnityEngine;

public abstract class EditorMazeTileAttributeModifier : IEditorMazeTileAttribute
{
    public virtual string Name => "";

    public Sprite Sprite => null;

    public virtual void PlaceAttribute(Tile tile)
    {
        throw new System.NotImplementedException();
    }

    public virtual void PlaceAttributeVariation(Tile tile)
    {
        Logger.Log("There are no attribute variations implemented for this attribute type");
    }

    public virtual Sprite GetSprite()
    {
        return EditorUIContainer.Instance.DefaultIcon;
    }
}