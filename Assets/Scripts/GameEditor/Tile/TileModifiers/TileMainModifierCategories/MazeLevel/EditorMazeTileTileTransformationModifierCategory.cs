using UnityEngine;

public class EditorMazeTileTileTransformationModifierCategory : EditorMazeTileMainModifierCategory
{
    public override string Name { get => "Transformation"; }
    private Sprite _sprite = EditorCanvasUI.Instance.TileMainModifierCategoryIcons[3];
    public override Sprite Sprite { get => _sprite; set => _sprite = value; }

    public override Sprite GetSprite()
    {
        return Sprite;
    }
}
