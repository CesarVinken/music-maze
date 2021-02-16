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
        EditorMazeTileModificationPanel.Instance.DestroyModifierActions();

        if (EditorManager.SelectedTileModifierCategory == EditorTileModifierCategory.Attribute)
        {
            SelectBackgrounds();
        }
        else if (EditorManager.SelectedTileModifierCategory == EditorTileModifierCategory.Background)
        {
            SelectTileTransformationTriggerer();
        }
        else if (EditorManager.SelectedTileModifierCategory == EditorTileModifierCategory.TransformationTriggerer)
        {
            SelectAttributes();
        }
        else
        {
            Logger.Error("Unknown modifier type");
        }
    }

    public void SelectNextModifierCategory()
    {
        EditorMazeTileModificationPanel.Instance.DestroyModifierActions();

        if (EditorManager.SelectedTileModifierCategory == EditorTileModifierCategory.Attribute)
        {
            SelectTileTransformationTriggerer();
        }
        else if (EditorManager.SelectedTileModifierCategory == EditorTileModifierCategory.Background)
        {
            SelectAttributes();
        }
        else if (EditorManager.SelectedTileModifierCategory == EditorTileModifierCategory.TransformationTriggerer)
        {
            SelectBackgrounds();
        }
        else
        {
            Logger.Error("Unknown modifier type");
        }
    }

    public void SelectAttributes()
    {
        EditorCanvasUI.Instance.SelectedTileModifierContainer.SetSelectedMazeTileModifierCategory(EditorTileModifierCategory.Attribute);

        int index = EditorManager.SelectedTileAttributeModifierIndex;
        EditorCanvasUI.Instance.SelectedTileModifierContainer.SetSelectedMazeTileModifier(index);
    }

    public void SelectBackgrounds()
    {
        EditorCanvasUI.Instance.SelectedTileModifierContainer.SetSelectedMazeTileModifierCategory(EditorTileModifierCategory.Background);

        int index = EditorManager.SelectedTileBackgroundModifierIndex;
        EditorCanvasUI.Instance.SelectedTileModifierContainer.SetSelectedMazeTileModifier(index);
    }

    public void SelectTileTransformationTriggerer()
    {
        EditorCanvasUI.Instance.SelectedTileModifierContainer.SetSelectedMazeTileModifierCategory(EditorTileModifierCategory.TransformationTriggerer);

        int index = EditorManager.SelectedTileBackgroundModifierIndex;
        EditorCanvasUI.Instance.SelectedTileModifierContainer.SetSelectedMazeTileModifier(index);
    }
}
