using UnityEngine;

public abstract class EditorMazeTileBackgroundModifier : EditorTileBackgroundModifier
{
    public override string Name { get; set; }

    public override Sprite Sprite { get; set; }

    public override void PlaceBackground<T>(T tile)
    {
        PlaceBackground(tile as EditorMazeTile);
    }

    public virtual void PlaceBackground(EditorMazeTile tile)
    { }

    public override void PlaceBackgroundVariation<T>(T tile)
    {
        PlaceBackgroundVariation(tile as EditorMazeTile);
    }

    public virtual void PlaceBackgroundVariation(EditorMazeTile tile)
    { }


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