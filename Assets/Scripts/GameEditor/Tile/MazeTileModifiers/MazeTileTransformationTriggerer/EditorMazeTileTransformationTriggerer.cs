using System.Linq;
using UnityEngine;
public abstract class EditorMazeTileTransformationTriggerer<T> : IEditorTileTransformationTriggerer<T> where T : EditorMazeTile
{
    public virtual string Name => throw new System.NotImplementedException();

    public virtual Sprite Sprite => throw new System.NotImplementedException();

    public virtual Sprite GetSprite()
    {
        throw new System.NotImplementedException();
    }

    public virtual void HandleBeautificationTriggerPlacement(T tile)
    {
        throw new System.NotImplementedException();
    }

    public virtual void InstantiateModifierActions()
    {
        throw new System.NotImplementedException();
    }
}

