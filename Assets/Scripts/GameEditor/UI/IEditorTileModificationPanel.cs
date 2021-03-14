using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface IEditorTileModificationPanel : IEditorModificationPanel
{
    [SerializeField] Transform TileModifierActionsContainer { get; set; }
    Dropdown SelectedMainMaterialDropdown { get; set; }

    List<EditorTileMainMaterialModifier> EditorTileMainMaterials { get; set; }

    void DestroyModifierActions();
}
