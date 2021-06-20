using System.Collections.Generic;


public class EditorTileAreaModifierSelector : EditorTileModifierSelector
{
    public EditorTileAreaModifierSelector(EditorSelectedTileModifierContainer editorSelectedModifierContainer) : base(editorSelectedModifierContainer) { }

    public override void SwitchSelectedModifier(int newValue)
    {
        int selectedModifierIndex = EditorManager.SelectedTileAreaModifierIndex;
        int newIndex = selectedModifierIndex + newValue;

        SwitchSelectedModifier(newIndex, EditorTileModifierCategory.Area);
        if (newIndex >= 0 && newIndex < _editorSelectedModifierContainer.CurrentlyAvailableTileModifiers[EditorTileModifierCategory.Area].Count)
        {
            SetSelectedModifier(newIndex);
        }
    }

    public override void SetSelectedModifier(int modifierIndex)
    {
        List<IEditorTileModifier> currentlyAvailableTileAreaModifiers = _editorSelectedModifierContainer.CurrentlyAvailableTileModifiers[EditorTileModifierCategory.Area];

        if (currentlyAvailableTileAreaModifiers.Count == 0)
        {
            EditorManager.SelectedTileAreaModifierIndex = 0;
            return;
        }

        EditorTileAreaModifier areaModifier = currentlyAvailableTileAreaModifiers[modifierIndex] as EditorTileAreaModifier;
        _editorSelectedModifierContainer.SelectedModifierLabel.text = GetSelectedModifierLabel(areaModifier.Name);
        _editorSelectedModifierContainer.SelectedModifierSprite.sprite = areaModifier.GetSprite();
        EditorManager.SelectedTileAreaModifierIndex = modifierIndex;

        areaModifier.InstantiateModifierActions();
    }
}
