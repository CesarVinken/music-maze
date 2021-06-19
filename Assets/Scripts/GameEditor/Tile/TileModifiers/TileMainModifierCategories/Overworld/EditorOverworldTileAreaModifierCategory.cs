using UnityEngine;

public class EditorOverworldTileAreaModifierCategory : EditorOverworldTileMainModifierCategory
{
    public override string Name => "Ground";

    private Sprite _sprite = EditorCanvasUI.Instance.TileMainModifierCategoryIcons[2];
    public override Sprite Sprite { get => _sprite; set => _sprite = value; }

    public override Sprite GetSprite()
    {
        return Sprite;
    }
}
