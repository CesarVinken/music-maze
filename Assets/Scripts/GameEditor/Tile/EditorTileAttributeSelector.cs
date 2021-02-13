using System.Collections.Generic;

public class EditorTileAttributeSelector : EditorTileModifierSelector
{
    public EditorTileAttributeSelector(EditorSelectedTileModifierContainer editorSelectedModifierContainer) : base(editorSelectedModifierContainer) { }

    public override void SwitchSelectedModifier(int newValue)
    {
        EditorTileModifierCategory currentCategory = EditorTileModifierCategory.Attribute;

        int selectedAttributeIndex = EditorManager.SelectedTileAttributeModifierIndex;
        int newIndex = selectedAttributeIndex + newValue;

        if (newIndex < 0)
        {
            EditorTileModifierCategory previousEditorTileModifierCategory = PreviousEditorTileModfierCategory(currentCategory);
            List<IEditorTileModifierType> registeredModifiers = EditorSelectedMazeTileModifierContainer.Instance.ModifiersByCategories[previousEditorTileModifierCategory];
            
            EditorSelectedMazeTileModifierContainer.Instance.SetSelectedMazeTileModifierCategory(previousEditorTileModifierCategory);
            EditorSelectedMazeTileModifierContainer.Instance.SetSelectedMazeTileModifier(registeredModifiers.Count - 1);
        }
        else if (newIndex >= _editorSelectedModifierContainer.EditorTileAttributes.Count)
        {
            EditorTileModifierCategory nextEditorTileModifierCategory = NextEditorTileModfierCategory(currentCategory);

            EditorSelectedMazeTileModifierContainer.Instance.SetSelectedMazeTileModifierCategory(nextEditorTileModifierCategory);
            EditorSelectedMazeTileModifierContainer.Instance.SetSelectedMazeTileModifier(0);
        }
        else
        {
            SetSelectedModifier(newIndex);
        }
    }

    public override void SetSelectedModifier(int modifierIndex)
    {
        IEditorTileAttribute attribute = _editorSelectedModifierContainer.EditorTileAttributes[modifierIndex];
        _editorSelectedModifierContainer.SelectedModifierLabel.text = GetSelectedModifierLabel(attribute.Name);
        _editorSelectedModifierContainer.SelectedModifierSprite.sprite = attribute.GetSprite();
        EditorManager.SelectedTileAttributeModifierIndex = modifierIndex;
    }
}
