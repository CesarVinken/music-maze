using UnityEngine;

public abstract class EditorMazeTileMainMaterialModifier : EditorTileMainMaterialModifier
{
    public override string Name { get; set; }
    private Sprite _sprite = EditorCanvasUI.Instance.DefaultIcon;
    public override Sprite Sprite { get => _sprite; set => _sprite = value; }

    public override Sprite GetSprite()
    {
        return Sprite;
    }

    public override void PlaceMainMaterial<T>(T tile)
    {
        PlaceMainMaterial(tile as EditorMazeTile);
    }

    public virtual void PlaceMainMaterial(EditorMazeTile tile)
    { }
}
