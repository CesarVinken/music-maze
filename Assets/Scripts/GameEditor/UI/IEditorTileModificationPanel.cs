using UnityEngine;

public interface IEditorTileModificationPanel : IEditorModificationPanel
{
    [SerializeField] Transform TileModifierActionsContainer { get; set; }
    void DestroyModifierActions();
}
