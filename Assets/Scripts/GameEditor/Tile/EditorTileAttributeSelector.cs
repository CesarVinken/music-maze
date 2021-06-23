using System.Collections.Generic;

public class EditorTileAttributeSelector : EditorTileModifierSelector
{
    public EditorTileAttributeSelector(EditorSelectedTileModifierContainer editorSelectedModifierContainer) : base(editorSelectedModifierContainer) { }

    public override void SwitchSelectedModifier(int newValue)
    {
        int selectedModifierIndex = EditorManager.SelectedTileAttributeModifierIndex;
        int newIndex = selectedModifierIndex + newValue;

        SwitchSelectedModifier(newIndex, EditorTileModifierCategory.Attribute);
        if (newIndex >= 0 && newIndex < _editorSelectedModifierContainer.CurrentlyAvailableTileModifiers[EditorTileModifierCategory.Attribute].Count)
        {
            SetSelectedModifier(newIndex);
        }
    }

    public override void SetSelectedModifier(int modifierIndex)
    {
        List<IEditorTileModifier> currentlyAvailableTileAttributes = _editorSelectedModifierContainer.CurrentlyAvailableTileModifiers[EditorTileModifierCategory.Attribute];
        
        if (currentlyAvailableTileAttributes.Count == 0)
        {
            EditorManager.SelectedTileAttributeModifierIndex = 0;
            return;
        }

        EditorTileAttributeModifier attribute = currentlyAvailableTileAttributes[modifierIndex] as EditorTileAttributeModifier;
        _editorSelectedModifierContainer.SelectedModifierLabel.text = GetSelectedModifierLabel(attribute.Name);
        _editorSelectedModifierContainer.SelectedModifierSprite.sprite = attribute.GetSprite();
        EditorManager.SelectedTileAttributeModifierIndex = modifierIndex;

        EditorManager.SelectedTileModifier = attribute;

        attribute.InstantiateModifierActions();
    }
}
