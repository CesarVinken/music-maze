using UnityEngine;
using UnityEngine.UI;

public interface IEditorTileModificationPanel : IEditorModificationPanel
{
    [SerializeField] Transform TileModifierActionsContainer { get; set; }
    Image SelectedMainMaterialIcon { get; set; }

    void SetMainMaterialIcon(Sprite selectedMainMaterialIcon);
    void DestroyModifierActions();
}
