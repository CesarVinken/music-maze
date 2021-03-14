using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Dropdown;

public class EditorOverworldTileModificationPanel : MonoBehaviour, IEditorTileModificationPanel
{
    public static EditorOverworldTileModificationPanel Instance;

    [SerializeField] private Transform _tileModifierActionsContainer;
    [SerializeField] private Dropdown _selectedMainMaterialDropdown;
    [SerializeField] private List<EditorTileMainMaterialModifier> _editorTileMainMaterials = new List<EditorTileMainMaterialModifier>();

    public List<EditorTileMainMaterialModifier> EditorTileMainMaterials { get => _editorTileMainMaterials; set => _editorTileMainMaterials = value; }
    public Transform TileModifierActionsContainer { get => _tileModifierActionsContainer; set => _tileModifierActionsContainer = value; }
    public Dropdown SelectedMainMaterialDropdown { get => _selectedMainMaterialDropdown; set => _selectedMainMaterialDropdown = value; }

    public void Awake()
    {
        Instance = this;

        Guard.CheckIsNull(TileModifierActionsContainer, "TileModifierActionsContainer");
    }

    public void Start()
    {
        EditorTileMainMaterials.Add(new EditorMazeTileGroundMaterial());
        EditorTileMainMaterials.Add(new EditorMazeTileWaterMaterial());

        EditorManager.SelectedTileMainMaterialModifierIndex = 0;   // set to Ground material by default

        InitialiseDropdown();
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

    private void InitialiseDropdown()
    {
        _selectedMainMaterialDropdown.ClearOptions();
        _selectedMainMaterialDropdown.onValueChanged.AddListener(delegate {
            DropdownValueChanged(_selectedMainMaterialDropdown);
        });

        List<OptionData> options = new List<OptionData>();

        for (int i = 0; i < EditorTileMainMaterials.Count; i++)
        {
            EditorTileMainMaterialModifier mainMaterialModifier = EditorTileMainMaterials[i];
            options.Add(new OptionData(mainMaterialModifier.Sprite));
        }
        _selectedMainMaterialDropdown.AddOptions(options);
    }

    private void DropdownValueChanged(Dropdown change)
    {
        EditorTileMainMaterialModifier mainMaterialModifier = EditorTileMainMaterials[change.value];
        Logger.Warning("New Value : " + mainMaterialModifier.Name);

        EditorManager.SelectedTileMainMaterialModifierIndex = change.value;

        EditorCanvasUI.Instance.SelectedTileModifierContainer.SetCurrentlyAvailableModifiers(mainMaterialModifier);
    }
}
