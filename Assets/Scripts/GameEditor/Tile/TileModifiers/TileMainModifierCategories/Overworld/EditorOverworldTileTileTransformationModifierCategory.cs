using UnityEngine;

public class EditorOverworldTileTileTransformationModifierCategory : EditorOverworldTileMainModifierCategory
{
    public override string Name => "Ground";

    private Sprite _sprite = EditorCanvasUI.Instance.TileMainModifierCategoryIcons[3];
    public override Sprite Sprite { get => _sprite; set => _sprite = value; }

    public override Sprite GetSprite()
    {
        return Sprite;
    }
}
