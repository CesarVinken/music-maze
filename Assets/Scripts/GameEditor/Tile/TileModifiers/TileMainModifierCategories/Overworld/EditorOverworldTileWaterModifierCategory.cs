using UnityEngine;

public class EditorOverworldTileWaterModifierCategory : EditorOverworldTileMainModifierCategory, IWaterMaterialModifier
{
    public override string Name { get => "Water"; }
    private Sprite _sprite = EditorCanvasUI.Instance.TileMainModifierCategoryIcons[1];
    public override Sprite Sprite { get => _sprite; set => _sprite = value; }

    public override Sprite GetSprite()
    {
        return Sprite;
    }
}
