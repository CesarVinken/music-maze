using UnityEngine;

public class EditorSelectedMazeTileModifierContainer : EditorSelectedTileModifierContainer
{
    void Awake()
    {
        Guard.CheckIsNull(SelectedModifierLabelGO, "SelectedModifierLabelGO", gameObject);
        Guard.CheckIsNull(SelectedModifierSpriteGO, "SelectedModifierSpriteGO", gameObject);
        Guard.CheckIsNull(SelectedModifierLabel, "SelectedModifierLabel", gameObject);
        Guard.CheckIsNull(SelectedModifierSprite, "SelectedModifierSprite", gameObject);

        EditorCanvasUI.Instance.SelectedTileModifierContainer = this;

        ResetLists();

        _editorTileAttributeSelector = new EditorTileAttributeSelector(this);
        _editorTileBackgroundSelector = new EditorTileBackgroundSelector(this);
        _editorTileTransformationTriggererSelector = new EditorTileTransformationTriggererSelector(this);
        _editorTileAreaModifierSelector = new EditorTileAreaModifierSelector(this);

        EditorTileAttributes.Add(new EditorObstacleTileAttribute());
        EditorTileAttributes.Add(new EditorPlayerExitTileAttribute());
        EditorTileAttributes.Add(new EditorPlayerSpawnpointMazeTileAttribute());
        EditorTileAttributes.Add(new EditorPlayerOnlyTileAttribute() );
        EditorTileAttributes.Add(new EditorEnemySpawnpointTileAttribute());
        EditorTileAttributes.Add(new EditorBridgeMazeTileAttribute());
        EditorTileAttributes.Add(new EditorMusicInstrumentCaseTileAttribute());
        EditorTileAttributes.Add(new EditorSheetmusicTileAttribute());

        EditorManager.SelectedTileAttributeModifierIndex = 0;

        EditorTileBackgrounds.Add(new EditorMazeTileBaseGround());
        EditorTileBackgrounds.Add(new EditorMazeTileBaseWater());
        EditorTileBackgrounds.Add(new EditorMazeTilePath());
        EditorManager.SelectedTileBackgroundModifierIndex = 0;

        EditorTileTransformationTriggerers.Add(new EditorMazeTileBeautificationTriggerer());

        EditorManager.SelectedTileTransformationTriggererIndex = 0;

        EditorTileAreaModifiers.Add(new EditorMazeTileTileAreaModifier());

        EditorManager.SelectedTileAreaModifierIndex = 0;


        UsedTileModifierCategories.Add(EditorTileModifierCategory.Background);
        UsedTileModifierCategories.Add(EditorTileModifierCategory.Attribute);
        UsedTileModifierCategories.Add(EditorTileModifierCategory.TransformationTriggerer);
        UsedTileModifierCategories.Add(EditorTileModifierCategory.Area);

        SetCurrentlyAvailableModifierCategories(new EditorMazeTileGroundModifierCategory());
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
        if (EditorManager.SelectedTileModifierCategory == EditorTileModifierCategory.Background)
        {
            _editorTileBackgroundSelector.SwitchSelectedModifier(-1);
        }
        else if (EditorManager.SelectedTileModifierCategory == EditorTileModifierCategory.Attribute)
        {
            _editorTileAttributeSelector.SwitchSelectedModifier(-1);
        }
        else if (EditorManager.SelectedTileModifierCategory == EditorTileModifierCategory.TransformationTriggerer)
        {
            _editorTileTransformationTriggererSelector.SwitchSelectedModifier(-1);
        }
        else if (EditorManager.SelectedTileModifierCategory == EditorTileModifierCategory.Area)
        {
            _editorTileAreaModifierSelector.SwitchSelectedModifier(-1);
        }
        else
        {
            // Not known type
            Logger.Error("Unknown tile modifier type");
        }
    }

    public override void SelectNextTileModifier()
    {
        if (EditorManager.SelectedTileModifierCategory == EditorTileModifierCategory.Background)
        {
            _editorTileBackgroundSelector.SwitchSelectedModifier(1);
        }
        else if (EditorManager.SelectedTileModifierCategory == EditorTileModifierCategory.Attribute)
        {
            _editorTileAttributeSelector.SwitchSelectedModifier(1);
        }
        else if (EditorManager.SelectedTileModifierCategory == EditorTileModifierCategory.TransformationTriggerer)
        {
            _editorTileTransformationTriggererSelector.SwitchSelectedModifier(1);
        }
        else if (EditorManager.SelectedTileModifierCategory == EditorTileModifierCategory.Area)
        {
            _editorTileAreaModifierSelector.SwitchSelectedModifier(1);
        }
        else
        {
            // Not known type
            Logger.Error("Unknown tile modifier type");
        }
    }

    public void SetInitialModifierValues()
    {
        EditorManager.SelectedTileMainModifierCategoryIndex = 0;
        EditorCanvasUI.Instance.SelectedTileModifierContainer.SetCurrentlyAvailableModifierCategories(new EditorMazeTileGroundModifierCategory());

        SetSelectedTileModifierCategory(EditorTileModifierCategory.Background);
        SetSelectedTileModifier(0);//Set selected modifier to Background -> Path 
    }
}
