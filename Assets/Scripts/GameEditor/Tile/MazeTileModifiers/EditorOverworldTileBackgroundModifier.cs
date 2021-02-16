using UnityEngine;

public class EditorOverworldTileBackgroundModifier<T> : IEditorTileBackground<T> where T : EditorOverworldTile
{
    public virtual string Name => "";

    public Sprite Sprite => null;

    public virtual Sprite GetSprite()
    {
        return EditorCanvasUI.Instance.DefaultIcon;
    }

    public virtual void InstantiateModifierActions()
    {
        throw new System.NotImplementedException();
    }

    public virtual void PlaceBackground(T tile)
    {
        throw new System.NotImplementedException();
    }

    public virtual void PlaceBackgroundVariation(T tile)
    {
        throw new System.NotImplementedException();
    }
}
