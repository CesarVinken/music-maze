using UnityEngine;

public class EditorMazeTileAreaModifierCategory : EditorMazeTileMainModifierCategory
{
    public override string Name { get => "Area"; }
    private Sprite _sprite = EditorCanvasUI.Instance.TileMainModifierCategoryIcons[2];
    public override Sprite Sprite { get => _sprite; set => _sprite = value; }

    public override Sprite GetSprite()
    {
        return Sprite;
    }
}
