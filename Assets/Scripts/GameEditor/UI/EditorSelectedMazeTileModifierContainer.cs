using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EditorSelectedMazeTileModifierContainer : EditorSelectedTileModifierContainer
{
    private EditorTileAttributeSelector _editorMazeTileAttributeSelector;
    private EditorTileBackgroundSelector _editorMazeTileBackgroundSelector;
    private EditorTileTransformationTriggererSelector _editorMazeTileTransformationTriggererSelector;

    void Awake()
    {
        Guard.CheckIsNull(SelectedModifierLabelGO, "SelectedModifierLabelGO", gameObject);
        Guard.CheckIsNull(SelectedModifierSpriteGO, "SelectedModifierSpriteGO", gameObject);
        Guard.CheckIsNull(SelectedModifierLabel, "SelectedModifierLabel", gameObject);
        Guard.CheckIsNull(SelectedModifierSprite, "SelectedModifierSprite", gameObject);
        Logger.Warning("BABABA");

        EditorCanvasUI.Instance.SelectedTileModifierContainer = this;

        Reset();

        _editorMazeTileAttributeSelector = new EditorTileAttributeSelector(this);
        _editorMazeTileBackgroundSelector = new EditorTileBackgroundSelector(this);
        _editorMazeTileTransformationTriggererSelector = new EditorTileTransformationTriggererSelector(this);

        EditorTileAttributes.Add(new EditorObstacleTileAttribute() as IEditorTileAttribute<Tile>);
        EditorTileAttributes.Add(new EditorPlayerExitTileAttribute() as IEditorTileAttribute<Tile>);
        EditorTileAttributes.Add(new EditorPlayerSpawnpointTileAttribute() as IEditorTileAttribute<Tile>);
        EditorTileAttributes.Add(new EditorPlayerOnlyTileAttribute() as IEditorTileAttribute<Tile>);
        EditorTileAttributes.Add(new EditorEnemySpawnpointTileAttribute() as IEditorTileAttribute<Tile>);


        EditorManager.SelectedTileAttributeModifierIndex = 0;

        EditorTileBackgrounds.Add(new EditorMazeTilePath());
        EditorManager.SelectedTileBackgroundModifierIndex = 0;
        Logger.Warning("We are failing to add the tile backgrounds to the list");
        for (int i = 0; i < EditorTileBackgrounds.Count; i++)
        {
            Logger.Log(EditorTileBackgrounds[i].Name);
        }

        EditorTileTransformationTriggerers.Add(new EditorMazeTileBeautificationTriggerer() as IEditorTileTransformationTriggerer<Tile>);
        EditorManager.SelectedTileTransformationTriggererIndex = 0;

        UsedTileModifierCategories.Add(EditorTileModifierCategory.Background);
        UsedTileModifierCategories.Add(EditorTileModifierCategory.Attribute);
        UsedTileModifierCategories.Add(EditorTileModifierCategory.TransformationTriggerer);

        ModifiersByCategories.Add(EditorTileModifierCategory.Background, (EditorTileBackgrounds as IEnumerable<IEditorTileModifierType>).ToList());
        ModifiersByCategories.Add(EditorTileModifierCategory.Attribute, (EditorTileAttributes as IEnumerable<IEditorTileModifierType>).ToList());
        ModifiersByCategories.Add(EditorTileModifierCategory.TransformationTriggerer, (EditorTileTransformationTriggerers as IEnumerable<IEditorTileModifierType>).ToList());

        //ModifierSelectorsByType.Add(EditorTileModifierCategory.Attribute, _editorMazeTileAttributeSelector);
        //ModifierSelectorsByType.Add(EditorTileModifierCategory.Background, _editorMazeTileBackgroundSelector);
        //ModifierSelectorsByType.Add(EditorTileModifierCategory.TransformationTriggerer, _editorMazeTileTransformationTriggererSelector);
    }

    public void Start()
    {
        SetSelectedMazeTileModifierCategory(EditorTileModifierCategory.Background);
        SetSelectedMazeTileModifier(0);//Set selected modifier to Background -> Path 
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
            _editorMazeTileBackgroundSelector.SwitchSelectedModifier(-1);
        }
        else if (EditorManager.SelectedTileModifierCategory == EditorTileModifierCategory.Attribute)
        {
            _editorMazeTileAttributeSelector.SwitchSelectedModifier(-1);
        }
        else if (EditorManager.SelectedTileModifierCategory == EditorTileModifierCategory.TransformationTriggerer)
        {
            _editorMazeTileTransformationTriggererSelector.SwitchSelectedModifier(-1);
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
            _editorMazeTileBackgroundSelector.SwitchSelectedModifier(1);
        }
        else if (EditorManager.SelectedTileModifierCategory == EditorTileModifierCategory.Attribute)
        {
            _editorMazeTileAttributeSelector.SwitchSelectedModifier(1);
        }
        else if (EditorManager.SelectedTileModifierCategory == EditorTileModifierCategory.TransformationTriggerer)
        {
            _editorMazeTileTransformationTriggererSelector.SwitchSelectedModifier(1);
        }
        else
        {
            // Not known type
            Logger.Error("Unknown tile modifier type");
        }
    }

    public override void SetSelectedMazeTileModifierCategory(EditorTileModifierCategory editorMazeTileModifierCategory)
    {
        if (EditorManager.SelectedTileModifierCategory == EditorTileModifierCategory.TransformationTriggerer)
        {
            EditorMazeTileBeautificationTriggerer editorMazeTileTransformationTriggerer = EditorTileTransformationTriggerers[EditorManager.SelectedTileTransformationTriggererIndex] as EditorMazeTileBeautificationTriggerer;
            editorMazeTileTransformationTriggerer.UnsetSelectedTile();
        }

        EditorManager.SelectedTileModifierCategory = editorMazeTileModifierCategory;
        EditorSelectedTileModifierCategoryContainer.Instance.SetCategoryLabel(EditorManager.SelectedTileModifierCategory);
    }

    public override void SetSelectedMazeTileModifier(int modifierIndex)
    {
        if (EditorManager.SelectedTileModifierCategory == EditorTileModifierCategory.Attribute)
        {
            _editorMazeTileAttributeSelector.SetSelectedModifier(modifierIndex);
        }
        else if (EditorManager.SelectedTileModifierCategory == EditorTileModifierCategory.Background)
        {
            _editorMazeTileBackgroundSelector.SetSelectedModifier(modifierIndex);
        }
        else if (EditorManager.SelectedTileModifierCategory == EditorTileModifierCategory.TransformationTriggerer)
        {
            _editorMazeTileTransformationTriggererSelector.SetSelectedModifier(modifierIndex);
        }
        else
        {
            Logger.Error("Unknown modifier type");
        }
    }
}
