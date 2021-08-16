using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Dropdown;

public class EditorOverworldTileModificationPanel : MonoBehaviour, IEditorTileModificationPanel
{
    public static EditorOverworldTileModificationPanel Instance;

    [SerializeField] private Transform _tileModifierActionsContainer;
    [SerializeField] private Dropdown _selectedMainMaterialDropdown;
    [SerializeField] private List<EditorTileMainModifierCategory> _editorTileMainModifierCategories = new List<EditorTileMainModifierCategory>();

    public List<EditorTileMainModifierCategory> EditorTileMainModifierCategories { get => _editorTileMainModifierCategories; set => _editorTileMainModifierCategories = value; }
    public Transform TileModifierActionsContainer { get => _tileModifierActionsContainer; set => _tileModifierActionsContainer = value; }
    public Dropdown SelectedMainMaterialDropdown { get => _selectedMainMaterialDropdown; set => _selectedMainMaterialDropdown = value; }

    public void Awake()
    {
        Instance = this;

        Guard.CheckIsNull(TileModifierActionsContainer, "TileModifierActionsContainer");
    }

    public void Start()
    {
        EditorTileMainModifierCategories.Add(new EditorOverworldTileGroundModifierCategory());
        EditorTileMainModifierCategories.Add(new EditorOverworldTileWaterModifierCategory());
        //EditorTileMainModifierCategories.Add(new EditorOverworldTileAreaModifierCategory());

        EditorManager.SelectedTileMainModifierCategoryIndex = 0;   // set to Ground material by default

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

    public void Reset()
    {
        _selectedMainMaterialDropdown.value = 0;

        EditorTileMainModifierCategory mainModifierCategory = EditorTileMainModifierCategories[0];
        Logger.Warning("New Dropdown Value : " + mainModifierCategory.Name);

        EditorManager.SelectedTileMainModifierCategoryIndex = 0;

        EditorCanvasUI.Instance.SelectedTileModifierContainer.SetCurrentlyAvailableModifierCategories(mainModifierCategory);
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

        for (int i = 0; i < EditorTileMainModifierCategories.Count; i++)
        {
            EditorTileMainModifierCategory mainMaterialModifier = EditorTileMainModifierCategories[i];
            options.Add(new OptionData(mainMaterialModifier.Sprite));
        }
        _selectedMainMaterialDropdown.AddOptions(options);
    }

    private void DropdownValueChanged(Dropdown change)
    {
        EditorTileMainModifierCategory mainModifierCategory = EditorTileMainModifierCategories[change.value];
        Logger.Warning("New Value : " + mainModifierCategory.Name);

        EditorManager.SelectedTileMainModifierCategoryIndex = change.value;

        EditorCanvasUI.Instance.SelectedTileModifierContainer.SetCurrentlyAvailableModifierCategories(mainModifierCategory);
    }
}
