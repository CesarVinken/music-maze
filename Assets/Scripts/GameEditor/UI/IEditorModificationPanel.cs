using UnityEngine;

public interface IEditorTileModificationPanel : IEditorModificationPanel
{
    [SerializeField] Transform TileModifierActionsContainer { get; set; }
    void DestroyModifierActions();
}

public interface IEditorModificationPanel
{
    void Open();
    void Close();

}
