using UnityEngine;

public class EditorMazeTileTransformationTriggerer : IEditorMazeTileTransformationTriggerer
{
    public string Name { get => "Transformation Triggerer"; }

    public Sprite Sprite => throw new System.NotImplementedException();

    public Sprite GetSprite()
    {
        return SpriteManager.Instance.DefaultDoor[0];
    }
}
