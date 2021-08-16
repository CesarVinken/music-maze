using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Dropdown;

public class EditorMazeTileModificationPanel : MonoBehaviour, IEditorTileModificationPanel
{
    public static EditorMazeTileModificationPanel Instance;

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
        Guard.CheckIsNull(_selectedMainMaterialDropdown, "_selectedMainMaterialDropdown");
    }

    public void Start()
    {
        EditorTileMainModifierCategories.Add(new EditorMazeTileGroundModifierCategory());
        EditorTileMainModifierCategories.Add(new EditorMazeTileWaterModifierCategory());
        EditorTileMainModifierCategories.Add(new EditorMazeTileAreaModifierCategory());
        EditorTileMainModifierCategories.Add(new EditorMazeTileTileTransformationModifierCategory());

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
            EditorTileMainModifierCategory mainModifierCategory = EditorTileMainModifierCategories[i];
            options.Add(new OptionData(mainModifierCategory.Sprite));
        }
        _selectedMainMaterialDropdown.AddOptions(options);
    }

    private void DropdownValueChanged(Dropdown change)
    {
        if (EditorManager.SelectedTileMainModifierCategoryIndex == change.value) return;

        EditorTileMainModifierCategory mainModifierCategory = EditorTileMainModifierCategories[change.value];
        Logger.Warning("New Dropdown Value : " + mainModifierCategory.Name);

        EditorManager.SelectedTileMainModifierCategoryIndex = change.value;

        EditorCanvasUI.Instance.SelectedTileModifierContainer.SetCurrentlyAvailableModifierCategories(mainModifierCategory);
    }
}
