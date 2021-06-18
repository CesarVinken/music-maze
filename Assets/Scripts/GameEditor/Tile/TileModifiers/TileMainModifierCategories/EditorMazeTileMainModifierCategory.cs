using UnityEngine;

public abstract class EditorMazeTileMainModifierCategory : EditorTileMainModifierCategory
{
    public override string Name { get; set; }
    private Sprite _sprite = EditorCanvasUI.Instance.DefaultIcon;
    public override Sprite Sprite { get => _sprite; set => _sprite = value; }

    public override Sprite GetSprite()
    {
        return Sprite;
    }
}
