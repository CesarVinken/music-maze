using UnityEngine;
using UnityEngine.UI;

public class EditorOverworldTileModificationPanel : MonoBehaviour, IEditorTileModificationPanel
{
    public static EditorOverworldTileModificationPanel Instance;

    [SerializeField] private Transform _tileModifierActionsContainer;
    [SerializeField] private Image _selectedMainMaterialIcon;

    public Transform TileModifierActionsContainer { get => _tileModifierActionsContainer; set => _tileModifierActionsContainer = value; }
    public Image SelectedMainMaterialIcon { get => _selectedMainMaterialIcon; set => _selectedMainMaterialIcon = value; }

    public void Awake()
    {
        Instance = this;

        Guard.CheckIsNull(TileModifierActionsContainer, "TileModifierActionsContainer");
    }

    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void DestroyModifierActions()
    {
        foreach (Transform action in TileModifierActionsContainer)
        {
            GameObject.Destroy(action.gameObject);
        }
    }

    public void SetMainMaterialIcon(Sprite selectedMainMaterialIcon)
    {
        SelectedMainMaterialIcon.sprite = selectedMainMaterialIcon;
    }
}
