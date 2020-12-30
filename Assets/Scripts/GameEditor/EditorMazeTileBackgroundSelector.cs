public class EditorMazeTileBackgroundSelector : EditorMazeTileModifierSelector
{
    public EditorMazeTileBackgroundSelector(EditorSelectedModifierContainer editorSelectedModifierContainer) : base(editorSelectedModifierContainer) { }

    public override void SwitchSelectedModifier(int newValue)
    {
        int selectedBackgroundIndex = EditorManager.SelectedMazeTileBackgroundModifierIndex;
        int newIndex = selectedBackgroundIndex + newValue;

        if (newIndex < 0)
        {
            // switch from backgrounds to last TileAttribute, because TileAttributes come before Backgrounds
            EditorSelectedModifierContainer.Instance.SetSelectedMazeTileModifierType(EditorMazeTileModifierType.Attribute);
            EditorSelectedModifierContainer.Instance.SetSelectedMazeTileModifier(EditorSelectedModifierContainer.Instance.EditorMazeTileAttributes.Count - 1);
        }
        else if (newIndex >= _editorSelectedModifierContainer.EditorMazeTileBackgrounds.Count)
        {
            // switch from backgrounds to first TileAttribute, because TileAttributes come after Backgrounds
            EditorSelectedModifierContainer.Instance.SetSelectedMazeTileModifierType(EditorMazeTileModifierType.Attribute);
            EditorSelectedModifierContainer.Instance.SetSelectedMazeTileModifier(0);
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
