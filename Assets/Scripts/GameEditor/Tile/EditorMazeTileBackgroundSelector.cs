public class EditorMazeTileBackgroundSelector : EditorMazeTileModifierSelector
{
    public EditorMazeTileBackgroundSelector(EditorSelectedTileModifierContainer editorSelectedModifierContainer) : base(editorSelectedModifierContainer) { }

    public override void SwitchSelectedModifier(int newValue)
    {
        int selectedBackgroundIndex = EditorManager.SelectedMazeTileBackgroundModifierIndex;
        int newIndex = selectedBackgroundIndex + newValue;

        if (newIndex < 0)
        {
            // switch from backgrounds to last TransformTriggerer, because TransformTriggerer come before Backgrounds
            EditorSelectedTileModifierContainer.Instance.SetSelectedMazeTileModifierCategory(EditorMazeTileModifierType.TransformationTriggerer);
            EditorSelectedTileModifierContainer.Instance.SetSelectedMazeTileModifier(EditorSelectedTileModifierContainer.Instance.EditorMazeTileTransformationTriggerers.Count - 1);
        }
        else if (newIndex >= _editorSelectedModifierContainer.EditorMazeTileBackgrounds.Count)
        {
            // switch from backgrounds to first TileAttribute, because TileAttributes come after Backgrounds
            EditorSelectedTileModifierContainer.Instance.SetSelectedMazeTileModifierCategory(EditorMazeTileModifierType.Attribute);
            EditorSelectedTileModifierContainer.Instance.SetSelectedMazeTileModifier(0);
        }
        else
        {
            SetSelectedModifier(newIndex);
        }
    }

    public override void SetSelectedModifier(int modifierIndex)
    {
        IEditorMazeTileBackground background = _editorSelectedModifierContainer.EditorMazeTileBackgrounds[modifierIndex];
        _editorSelectedModifierContainer.SelectedModifierLabel.text = GetSelectedModifierLabel(background.Name);
        _editorSelectedModifierContainer.SelectedModifierSprite.sprite = background.GetSprite();
        EditorManager.SelectedMazeTileBackgroundModifierIndex = modifierIndex;
    }
}
