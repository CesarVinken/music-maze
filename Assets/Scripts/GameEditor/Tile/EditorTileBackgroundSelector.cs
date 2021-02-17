using System.Collections.Generic;

public class EditorTileBackgroundSelector : EditorTileModifierSelector
{
    public EditorTileBackgroundSelector(EditorSelectedTileModifierContainer editorSelectedModifierContainer) : base(editorSelectedModifierContainer) { }

    public override void SwitchSelectedModifier(int newValue)
    {
        EditorTileModifierCategory currentCategory = EditorTileModifierCategory.Background;

        int selectedBackgroundIndex = EditorManager.SelectedTileBackgroundModifierIndex;
        int newIndex = selectedBackgroundIndex + newValue;

        if (newIndex < 0)
        {
            EditorTileModifierCategory previousEditorTileModifierCategory = PreviousEditorTileModfierCategory(currentCategory);
            List<EditorTileModifier> registeredModifiers = EditorCanvasUI.Instance.SelectedTileModifierContainer.ModifiersByCategories[previousEditorTileModifierCategory];

            EditorCanvasUI.Instance.SelectedTileModifierContainer.SetSelectedMazeTileModifierCategory(previousEditorTileModifierCategory);
            EditorCanvasUI.Instance.SelectedTileModifierContainer.SetSelectedMazeTileModifier(registeredModifiers.Count - 1);
        }
        else if (newIndex >= _editorSelectedModifierContainer.EditorTileBackgrounds.Count)
        {
            EditorTileModifierCategory nextEditorTileModifierCategory = NextEditorTileModfierCategory(currentCategory);

            EditorCanvasUI.Instance.SelectedTileModifierContainer.SetSelectedMazeTileModifierCategory(nextEditorTileModifierCategory);
            EditorCanvasUI.Instance.SelectedTileModifierContainer.SetSelectedMazeTileModifier(0);
        }
        else
        {
            SetSelectedModifier(newIndex);
        }
    }

    public override void SetSelectedModifier(int modifierIndex)
    {
        EditorTileBackgroundModifier background = _editorSelectedModifierContainer.EditorTileBackgrounds[modifierIndex];
        _editorSelectedModifierContainer.SelectedModifierLabel.text = GetSelectedModifierLabel(background.Name);
        _editorSelectedModifierContainer.SelectedModifierSprite.sprite = background.GetSprite();
        EditorManager.SelectedTileBackgroundModifierIndex = modifierIndex;
    }
}
