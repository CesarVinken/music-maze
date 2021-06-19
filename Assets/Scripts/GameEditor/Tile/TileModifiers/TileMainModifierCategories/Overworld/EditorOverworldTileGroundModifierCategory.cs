using UnityEngine;

public class EditorOverworldTileGroundModifierCategory : EditorOverworldTileMainModifierCategory
{
    public override string Name => "Ground";

    private Sprite _sprite = EditorCanvasUI.Instance.TileMainModifierCategoryIcons[0];
    public override Sprite Sprite { get => _sprite; set => _sprite = value; }

    public override Sprite GetSprite()
    {
        return Sprite;
    }
}
