using System.Linq;
using UnityEngine;
public class EditorMazeTileTransformationTriggerer : EditorTileTransformationModifier
{
    public override string Name { get; set; }

    public override Sprite Sprite { get; set; }

    public override Sprite GetSprite()
    {
        return EditorCanvasUI.Instance.DefaultIcon;
    }

    public override void HandleBeautificationTriggerPlacement<T>(T tile)
    {
        HandleBeautificationTriggerPlacement(tile as EditorMazeTile);

    }
    public virtual void HandleBeautificationTriggerPlacement(EditorMazeTile tile)
    {
    }

    public override void InstantiateModifierActions()
    {
    }

    public override void DestroyModifierActions()
    {
    }

    public override void UnsetSelectedTile()
    {

    }
}

