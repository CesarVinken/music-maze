using UnityEngine;

public class EditorSelectedOverworldTileModifierContainer : EditorSelectedTileModifierContainer
{
    void Awake()
    {
        Guard.CheckIsNull(SelectedModifierLabelGO, "SelectedModifierLabelGO", gameObject);
        Guard.CheckIsNull(SelectedModifierSpriteGO, "SelectedModifierSpriteGO", gameObject);
        Guard.CheckIsNull(SelectedModifierLabel, "SelectedModifierLabel", gameObject);
        Guard.CheckIsNull(SelectedModifierSprite, "SelectedModifierSprite", gameObject);

        EditorCanvasUI.Instance.SelectedTileModifierContainer = this;

        Reset();

        _editorTileMainMaterialSelector = new EditorTileMainMaterialSelector(this);
        _editorTileAttributeSelector = new EditorTileAttributeSelector(this);
        _editorTileBackgroundSelector = new EditorTileBackgroundSelector(this);
        _editorTileTransformationTriggererSelector = new EditorTileTransformationTriggererSelector(this);

        EditorTileMainMaterials.Add(new EditorOverworldGroundMaterial());

        EditorManager.SelectedTileMainMaterialModifierIndex = 0;
        
        EditorTileAttributes.Add(new EditorPlayerSpawnpointOverworldTileAttribute());
        EditorTileAttributes.Add(new EditorOverworldMazeLevelEntryTileAttribute());

        EditorManager.SelectedTileAttributeModifierIndex = 0;

        EditorTileBackgrounds.Add(new EditorOverworldTilePath());
        EditorManager.SelectedTileBackgroundModifierIndex = 0;

        EditorManager.SelectedTileTransformationTriggererIndex = 0;

        UsedTileModifierCategories.Add(EditorTileModifierCategory.MainMaterial);
        UsedTileModifierCategories.Add(EditorTileModifierCategory.Background);
        UsedTileModifierCategories.Add(EditorTileModifierCategory.Attribute);

        SetCurrentlyAvailableModifiers();
    }

    public void Start()
    {
        SetInitialModifierValues();
    }

    private void Update()
    {
        if (!EditorManager.InEditor) return;

        if (Input.GetKeyDown(KeyCode.PageDown))
        {
            SelectNextTileModifier();
        }
        else if (Input.GetKeyDown(KeyCode.PageUp))
        {
            SelectPreviousTileModifier();
        }
    }

    public override void SelectPreviousTileModifier()
    {
        if (EditorManager.SelectedTileModifierCategory == EditorTileModifierCategory.MainMaterial)
        {
            _editorTileMainMaterialSelector.SwitchSelectedModifier(-1);
        }
        else if (EditorManager.SelectedTileModifierCategory == EditorTileModifierCategory.Background)
        {
            _editorTileBackgroundSelector.SwitchSelectedModifier(-1);
        }
        else if (EditorManager.SelectedTileModifierCategory == EditorTileModifierCategory.Attribute)
        {
            _editorTileAttributeSelector.SwitchSelectedModifier(-1);
        }
        else
        {
            // Not known type
            Logger.Error("Unknown tile modifier type");
        }
    }

    public override void SelectNextTileModifier()
    {
        if (EditorManager.SelectedTileModifierCategory == EditorTileModifierCategory.MainMaterial)
        {
            _editorTileMainMaterialSelector.SwitchSelectedModifier(1);
        }
        else if (EditorManager.SelectedTileModifierCategory == EditorTileModifierCategory.Background)
        {
            _editorTileBackgroundSelector.SwitchSelectedModifier(1);
        }
        else if (EditorManager.SelectedTileModifierCategory == EditorTileModifierCategory.Attribute)
        {
            _editorTileAttributeSelector.SwitchSelectedModifier(1);
        }
        else
        {
            // Not known type
            Logger.Error("Unknown tile modifier type");
        }
    }

    private void SetInitialModifierValues()
    {
        _editorTileMainMaterialSelector.SetSelectedModifier(0);     // set to Ground material by default

        SetSelectedTileModifierCategory(EditorTileModifierCategory.Background);
        SetSelectedTileModifier(0);//Set selected modifier to Background -> Path 
    }
}
