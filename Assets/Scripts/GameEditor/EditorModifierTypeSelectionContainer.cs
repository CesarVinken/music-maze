using UnityEngine;

public class EditorModifierTypeSelectionContainer : MonoBehaviour
{
    public void SelectAttributes()
    {
        EditorSelectedModifierContainer.Instance.SetSelectedMazeTileModifierType(EditorMazeTileModifierType.Attribute);

        int index = EditorManager.SelectedMazeTileAttributeModifierIndex;
        EditorSelectedModifierContainer.Instance.SetSelectedMazeTileModifier(EditorSelectedModifierContainer.Instance.EditorMazeTileAttributes[index]);
    }

    public void SelectBackgrounds()
    {
        EditorSelectedModifierContainer.Instance.SetSelectedMazeTileModifierType(EditorMazeTileModifierType.Background);

        int index = EditorManager.SelectedMazeTileBackgroundModifierIndex;
        EditorSelectedModifierContainer.Instance.SetSelectedMazeTileModifier(EditorSelectedModifierContainer.Instance.EditorMazeTileBackgrounds[index]);
    }
}
