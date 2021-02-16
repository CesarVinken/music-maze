using UnityEngine;

public abstract class EditorMazeTileAttributeModifier<T> : IEditorTileAttribute<T> where T : EditorMazeTile
{
    public virtual string Name => "";

    public Sprite Sprite => null;

    public virtual void PlaceAttribute(T tile)
    {
        throw new System.NotImplementedException();
    }

    public virtual void PlaceAttributeVariation(T tile)
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