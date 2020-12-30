using UnityEngine;

public class EditorModifierTypeSelectionContainer : MonoBehaviour
{
    public static EditorModifierTypeSelectionContainer Instance;

    [SerializeField] private GameObject _attributesSelectionImage;
    [SerializeField] private GameObject _backgroundsSelectionImage;

    public void Awake()
    {
        Instance = this;

        Guard.CheckIsNull(_attributesSelectionImage, "_attributesSelectionImage", gameObject);
        Guard.CheckIsNull(_backgroundsSelectionImage, "_backgroundsSelectionImage", gameObject);
    }

    public void OnDisable()
    {
        _attributesSelectionImage.SetActive(false);
        _backgroundsSelectionImage.SetActive(false);
    }

    public void OnEnable()
    {
        if (EditorManager.SelectedMazeTileModifierType == EditorMazeTileModifierType.Attribute)
        {
            EnableAttributesSelectionImage();
        }
        else if (EditorManager.SelectedMazeTileModifierType == EditorMazeTileModifierType.Background)
        {
            EnableBackgroundsSelectionImage();
        }
        else
        {
            Logger.Error("Unknown modifier type");
        }
    }

    public void SelectAttributes()
    {
        EditorSelectedModifierContainer.Instance.SetSelectedMazeTileModifierType(EditorMazeTileModifierType.Attribute);

        int index = EditorManager.SelectedMazeTileAttributeModifierIndex;
        EditorSelectedModifierContainer.Instance.SetSelectedMazeTileModifier(index);
    }

    public void SelectBackgrounds()
    {
        EditorSelectedModifierContainer.Instance.SetSelectedMazeTileModifierType(EditorMazeTileModifierType.Background);

        int index = EditorManager.SelectedMazeTileBackgroundModifierIndex;
        EditorSelectedModifierContainer.Instance.SetSelectedMazeTileModifier(index);
    }

    public void EnableBackgroundsSelectionImage()
    {
        _backgroundsSelectionImage.SetActive(true);
        _attributesSelectionImage.SetActive(false);
    }

    public void EnableAttributesSelectionImage()
    {
        _attributesSelectionImage.SetActive(true);
        _backgroundsSelectionImage.SetActive(false);
    }
}
