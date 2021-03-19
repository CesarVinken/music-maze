using UnityEngine;

public class EditorOverworldTileBackgroundModifier : EditorTileBackgroundModifier
{
    public override string Name { get; set; }

    public override Sprite Sprite { get; set; }

    public override void PlaceBackground<T>(T tile)
    {
        PlaceBackground(tile as EditorOverworldTile);
    }

    public virtual void PlaceBackground(EditorOverworldTile tile)
    { }

    public override void PlaceBackgroundVariation<T>(T tile)
    {
        PlaceBackgroundVariation(tile as EditorOverworldTile);
    }

    public virtual void PlaceBackgroundVariation(EditorOverworldTile tile)
    {
        Logger.Log("There are no background variations implemented for this background type");
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
