using System.Collections.Generic;

public class EditorTileBackgroundSelector : EditorTileModifierSelector
{
    public EditorTileBackgroundSelector(EditorSelectedTileModifierContainer editorSelectedModifierContainer) : base(editorSelectedModifierContainer) { }

    public override void SwitchSelectedModifier(int newValue)
    {
        int selectedModifierIndex = EditorManager.SelectedTileBackgroundModifierIndex;
        int newIndex = selectedModifierIndex + newValue;

        SwitchSelectedModifier(newIndex, EditorTileModifierCategory.Background);
        if (newIndex >= 0 && newIndex < _editorSelectedModifierContainer.CurrentlyAvailableTileModifiers[EditorTileModifierCategory.Background].Count)
        {
            SetSelectedModifier(newIndex);
        }
    }

    public override void SetSelectedModifier(int modifierIndex)
    {
        List<IEditorTileModifier> currentlyAvailableTileBackgrounds = _editorSelectedModifierContainer.CurrentlyAvailableTileModifiers[EditorTileModifierCategory.Background];

        if (currentlyAvailableTileBackgrounds.Count == 0)
        {
            EditorManager.SelectedTileBackgroundModifierIndex = 0;
            return;
        }

        EditorTileBackgroundModifier background = currentlyAvailableTileBackgrounds[modifierIndex] as EditorTileBackgroundModifier;
        _editorSelectedModifierContainer.SelectedModifierLabel.text = GetSelectedModifierLabel(background.Name);
        _editorSelectedModifierContainer.SelectedModifierSprite.sprite = background.GetSprite();
        EditorManager.SelectedTileBackgroundModifierIndex = modifierIndex;
    }
}
