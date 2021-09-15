using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class EditorSelectedTileModifierCategoryContainer : MonoBehaviour
{
    public static EditorSelectedTileModifierCategoryContainer Instance;

    public Text SelectedTileModifierCategoryLabel;

    public void Awake()
    {
        Guard.CheckIsNull(SelectedTileModifierCategoryLabel, "SelectedTileModifierCategoryLabel");

        Instance = this;
    }

    public void OnEnable()
    {
        SetCategoryLabel(EditorManager.SelectedTileModifierCategory);
    }

    public void SetCategoryLabel(EditorTileModifierCategory editorMazeTileModifierType)
    {
        string categoryName = editorMazeTileModifierType.ToString();
        categoryName = Regex.Replace(categoryName, "([a-z])([A-Z])", "$1 $2");

        SelectedTileModifierCategoryLabel.text = categoryName;
    }

    public void SelectPreviousModifierCategory()
    {
        if (EditorCanvasUI.Instance.SelectedTileModifierContainer.UsedTileModifierCategories.Count == 1)
        {
            return;
        }

        EditorTileSelector.Instance.ResetColouredTiles();

        EditorTileModifierCategory currentCategory = EditorManager.SelectedTileModifierCategory;
        EditorSelectedTileModifierContainer selectedTileModifierContainer = EditorCanvasUI.Instance.SelectedTileModifierContainer;

        if (EditorModificationPanelContainer.Instance.SelectedPanel is IEditorTileModificationPanel)
        {
            IEditorTileModificationPanel selectedPanel = EditorModificationPanelContainer.Instance.SelectedPanel as IEditorTileModificationPanel;
            selectedPanel.DestroyModifierActions();
        }

        EditorTileModifierCategory previousEditorTileModifierCategory = PreviousEditorTileModifierCategory(currentCategory);

        bool landedAtCategory = false;

        while (!landedAtCategory)
        {
            if (selectedTileModifierContainer.CurrentlyAvailableTileModifiers.TryGetValue(previousEditorTileModifierCategory, out List<IEditorTileModifier> editorTileModifiers))
            {
                selectedTileModifierContainer.SetSelectedTileModifierCategory(previousEditorTileModifierCategory);
                selectedTileModifierContainer.SetSelectedTileModifier(0);
                landedAtCategory = true;
            }
            else
            {
                // there are no modifiers in the previous category. Try the category before that
                EditorTileModifierCategory lastQueriedModifierCategory = previousEditorTileModifierCategory;
                previousEditorTileModifierCategory = PreviousEditorTileModifierCategory(lastQueriedModifierCategory);
            }
        }
    }

    public void SelectNextModifierCategory()
    {
        if (EditorCanvasUI.Instance.SelectedTileModifierContainer.UsedTileModifierCategories.Count == 1)
        {
            return;
        }

        EditorTileSelector.Instance.ResetColouredTiles();

        EditorTileModifierCategory currentCategory = EditorManager.SelectedTileModifierCategory;
        EditorSelectedTileModifierContainer selectedTileModifierContainer = EditorCanvasUI.Instance.SelectedTileModifierContainer;


        if (EditorModificationPanelContainer.Instance.SelectedPanel is IEditorTileModificationPanel)
        {
            IEditorTileModificationPanel selectedPanel = EditorModificationPanelContainer.Instance.SelectedPanel as IEditorTileModificationPanel;
            selectedPanel.DestroyModifierActions();
        }

        EditorTileModifierCategory nextEditorTileModifierCategory = NextEditorTileModifierCategory(currentCategory);

        bool landedAtCategory = false;

        while (!landedAtCategory)
        {
            if (selectedTileModifierContainer.CurrentlyAvailableTileModifiers.TryGetValue(nextEditorTileModifierCategory, out List<IEditorTileModifier> editorTileModifiers))
            {
                selectedTileModifierContainer.SetSelectedTileModifierCategory(nextEditorTileModifierCategory);
                selectedTileModifierContainer.SetSelectedTileModifier(0);
                landedAtCategory = true;
            }
            else
            {
                // there are no modifiers in the next category. Try the category after that
                EditorTileModifierCategory lastQueriedModifierCategory = nextEditorTileModifierCategory;
                nextEditorTileModifierCategory = NextEditorTileModifierCategory(lastQueriedModifierCategory);
            }
        }
    }

    protected EditorTileModifierCategory PreviousEditorTileModifierCategory(EditorTileModifierCategory currentTileModifierCategory)
    {
        EditorSelectedTileModifierContainer selectedTileModifierContainer = EditorCanvasUI.Instance.SelectedTileModifierContainer;

        int index = selectedTileModifierContainer.UsedTileModifierCategories.IndexOf(currentTileModifierCategory);

        if (index > 0)
        {
            return selectedTileModifierContainer.UsedTileModifierCategories[index - 1];
        }
        return selectedTileModifierContainer.UsedTileModifierCategories[selectedTileModifierContainer.UsedTileModifierCategories.Count - 1];
    }

    protected EditorTileModifierCategory NextEditorTileModifierCategory(EditorTileModifierCategory currentTileModifierCategory)
    {
        EditorSelectedTileModifierContainer selectedTileModifierContainer = EditorCanvasUI.Instance.SelectedTileModifierContainer;

        int index = selectedTileModifierContainer.UsedTileModifierCategories.IndexOf(currentTileModifierCategory);

        if (index < selectedTileModifierContainer.UsedTileModifierCategories.Count - 1)
        {
            return selectedTileModifierContainer.UsedTileModifierCategories[index + 1];
        }
        return selectedTileModifierContainer.UsedTileModifierCategories[0];
    }
}
