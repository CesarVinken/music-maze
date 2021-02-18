using UnityEngine;

public class EditorOverworldTileAttributeModifier : EditorTileAttributeModifier
{
    public override string Name { get; set; }
    public override Sprite Sprite { get; set; }

    public override void PlaceAttribute<T>(T tile)
    {
        PlaceAttribute(tile as EditorOverworldTile);
    }

    public virtual void PlaceAttribute(EditorOverworldTile tile)
    {

    }

    public override void PlaceAttributeVariation<T>(T tile)
    {
        PlaceAttributeVariation(tile as EditorOverworldTile);
    }

    public virtual void PlaceAttributeVariation(EditorOverworldTile tile)
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
