using UnityEngine;

public abstract class EditorOverworldTileMainMaterialModifier : EditorTileMainMaterialModifier
{
    public override string Name { get; set; }
    public override Sprite Sprite { get; set; }

    public override Sprite GetSprite()
    {
        return EditorCanvasUI.Instance.DefaultIcon;
    }

    public override void PlaceMainMaterial<T>(T tile)
    {
        PlaceMainMaterial(tile as EditorOverworldTile);
    }

    public virtual void PlaceMainMaterial(EditorOverworldTile tile)
    { }

}
