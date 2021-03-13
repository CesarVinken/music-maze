using UnityEngine;

public abstract class EditorMazeTileAttributeModifier : EditorTileAttributeModifier
{
    public override string Name { get; set; }

    public override Sprite Sprite { get; set; }

    public override void PlaceAttribute<T>(T tile)
    {
        PlaceAttribute(tile as EditorMazeTile);
    }

    public virtual void PlaceAttribute(EditorMazeTile tile)
    {

    }

    public override void PlaceAttributeVariation<T>(T tile)
    {
        PlaceAttributeVariation(tile as EditorMazeTile);
    }

    public virtual void PlaceAttributeVariation(EditorMazeTile tile)
    {
        Logger.Log("There are no attribute variations implemented for this attribute type");
    }

    public override Sprite GetSprite()
    {
        return EditorCanvasUI.Instance.DefaultIcon;
    }

    public override void InstantiateModifierActions()
    {
    }

    public override void DestroyModifierActions()
    {
    }
}