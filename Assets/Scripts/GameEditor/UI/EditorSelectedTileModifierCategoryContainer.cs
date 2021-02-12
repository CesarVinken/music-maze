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
        SetCategoryLabel(EditorManager.SelectedMazeTileModifierCategory);
    }

    public void SetCategoryLabel(EditorMazeTileModifierType editorMazeTileModifierType)
    {
        string categoryName = editorMazeTileModifierType.ToString();
        categoryName = Regex.Replace(categoryName, "([a-z])([A-Z])", "$1 $2");

        SelectedTileModifierCategoryLabel.text = categoryName;
    }

    public void SelectPreviousModifierCategory()
    {
        EditorMazeTileModificationPanel.Instance.DestroyModifierActions();

        if (EditorManager.SelectedMazeTileModifierCategory == EditorMazeTileModifierType.Attribute)
        {
            SelectBackgrounds();
        }
        else if (EditorManager.SelectedMazeTileModifierCategory == EditorMazeTileModifierType.Background)
        {
            SelectTileTransformationTriggerer();
        }
        else if (EditorManager.SelectedMazeTileModifierCategory == EditorMazeTileModifierType.TransformationTriggerer)
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

        if (EditorManager.SelectedMazeTileModifierCategory == EditorMazeTileModifierType.Attribute)
        {
            SelectTileTransformationTriggerer();
        }
        else if (EditorManager.SelectedMazeTileModifierCategory == EditorMazeTileModifierType.Background)
        {
            SelectAttributes();
        }
        else if (EditorManager.SelectedMazeTileModifierCategory == EditorMazeTileModifierType.TransformationTriggerer)
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
        EditorSelectedTileModifierContainer.Instance.SetSelectedMazeTileModifierCategory(EditorMazeTileModifierType.Attribute);

        int index = EditorManager.SelectedMazeTileAttributeModifierIndex;
        EditorSelectedTileModifierContainer.Instance.SetSelectedMazeTileModifier(index);
    }

    public void SelectBackgrounds()
    {
        EditorSelectedTileModifierContainer.Instance.SetSelectedMazeTileModifierCategory(EditorMazeTileModifierType.Background);

        int index = EditorManager.SelectedMazeTileBackgroundModifierIndex;
        EditorSelectedTileModifierContainer.Instance.SetSelectedMazeTileModifier(index);
    }

    public void SelectTileTransformationTriggerer()
    {
        EditorSelectedTileModifierContainer.Instance.SetSelectedMazeTileModifierCategory(EditorMazeTileModifierType.TransformationTriggerer);

        int index = EditorManager.SelectedMazeTileBackgroundModifierIndex;
        EditorSelectedTileModifierContainer.Instance.SetSelectedMazeTileModifier(index);
    }
}
