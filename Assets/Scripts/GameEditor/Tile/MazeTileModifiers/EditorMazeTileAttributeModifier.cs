using UnityEngine;

public abstract class EditorMazeTileAttributeModifier : IEditorTileAttribute
{
    public virtual string Name => "";

    public Sprite Sprite => null;

    public virtual void PlaceAttribute(EditorMazeTile tile)
    {
        throw new System.NotImplementedException();
    }

    public virtual void PlaceAttributeVariation(EditorMazeTile tile)
    {
        Logger.Log("There are no attribute variations implemented for this attribute type");
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