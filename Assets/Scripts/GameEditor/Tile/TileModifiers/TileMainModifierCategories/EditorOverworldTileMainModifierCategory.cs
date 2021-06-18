using UnityEngine;

public abstract class EditorOverworldTileMainModifierCategory : EditorTileMainModifierCategory
{
    public override string Name { get; set; }
    public override Sprite Sprite { get; set; }

    public override Sprite GetSprite()
    {
        return EditorCanvasUI.Instance.DefaultIcon;
    }
}
