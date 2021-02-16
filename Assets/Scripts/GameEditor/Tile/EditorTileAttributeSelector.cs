using System.Collections.Generic;

public class EditorTileAttributeSelector : EditorTileModifierSelector
{
    public EditorTileAttributeSelector(EditorSelectedTileModifierContainer editorSelectedModifierContainer) : base(editorSelectedModifierContainer) { }

    public override void SwitchSelectedModifier(int newValue)
    {
        EditorTileModifierCategory currentCategory = EditorTileModifierCategory.Attribute;
        EditorSelectedTileModifierContainer selectedTileModifierContainer = EditorCanvasUI.Instance.SelectedTileModifierContainer;

        int selectedAttributeIndex = EditorManager.SelectedTileAttributeModifierIndex;
        int newIndex = selectedAttributeIndex + newValue;

        if (newIndex < 0)
        {
            EditorTileModifierCategory previousEditorTileModifierCategory = PreviousEditorTileModfierCategory(currentCategory);
            Logger.Log($"The previous category is {previousEditorTileModifierCategory}");
            List<IEditorTileModifierType> registeredModifiers = selectedTileModifierContainer.ModifiersByCategories[previousEditorTileModifierCategory];

            selectedTileModifierContainer.SetSelectedMazeTileModifierCategory(previousEditorTileModifierCategory);
            selectedTileModifierContainer.SetSelectedMazeTileModifier(registeredModifiers.Count - 1);
        }
        else if (newIndex >= _editorSelectedModifierContainer.EditorTileAttributes.Count)
        {
            EditorTileModifierCategory nextEditorTileModifierCategory = NextEditorTileModfierCategory(currentCategory);

            selectedTileModifierContainer.SetSelectedMazeTileModifierCategory(nextEditorTileModifierCategory);
            selectedTileModifierContainer.SetSelectedMazeTileModifier(0);
        }
        else
        {
            SetSelectedModifier(newIndex);
        }
    }

    public override void SetSelectedModifier(int modifierIndex)
    {
        if (_editorSelectedModifierContainer.EditorTileAttributes.Count == 0) return;

        IEditorTileAttribute<Tile> attribute = _editorSelectedModifierContainer.EditorTileAttributes[modifierIndex];
        _editorSelectedModifierContainer.SelectedModifierLabel.text = GetSelectedModifierLabel(attribute.Name);
        _editorSelectedModifierContainer.SelectedModifierSprite.sprite = attribute.GetSprite();
        EditorManager.SelectedTileAttributeModifierIndex = modifierIndex;
    }
}
