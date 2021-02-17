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

        EditorCanvasUI.Instance.SelectedTileModifierContainer = this;

        Reset();

        _editorMazeTileAttributeSelector = new EditorTileAttributeSelector(this);
        _editorMazeTileBackgroundSelector = new EditorTileBackgroundSelector(this);
        _editorMazeTileTransformationTriggererSelector = new EditorTileTransformationTriggererSelector(this);

        EditorTileAttributes.Add(new EditorObstacleTileAttribute());
        EditorTileAttributes.Add(new EditorPlayerExitTileAttribute());
        EditorTileAttributes.Add(new EditorPlayerSpawnpointTileAttribute());
        EditorTileAttributes.Add(new EditorPlayerOnlyTileAttribute() );
        EditorTileAttributes.Add(new EditorEnemySpawnpointTileAttribute());

        EditorManager.SelectedTileAttributeModifierIndex = 0;

        EditorTileBackgrounds.Add(new EditorMazeTilePath());
        EditorManager.SelectedTileBackgroundModifierIndex = 0;

        Logger.Warning("We are failing to add the tile backgrounds to the list");
        for (int i = 0; i < EditorTileBackgrounds.Count; i++)
        {
            Logger.Log(EditorTileBackgrounds[i].Name);
        }

        EditorTileTransformationTriggerers.Add(new EditorMazeTileBeautificationTriggerer());

        EditorManager.SelectedTileTransformationTriggererIndex = 0;

        UsedTileModifierCategories.Add(EditorTileModifierCategory.Background);
        UsedTileModifierCategories.Add(EditorTileModifierCategory.Attribute);
        UsedTileModifierCategories.Add(EditorTileModifierCategory.TransformationTriggerer);

        //ModifiersByCategories.Add(EditorTileModifierCategory.Background, (EditorTileBackgrounds as IEnumerable<EditorTileModifier>).ToList());
        //ModifiersByCategories.Add(EditorTileModifierCategory.Attribute, (EditorTileAttributes as IEnumerable<EditorTileModifier>).ToList());
        //ModifiersByCategories.Add(EditorTileModifierCategory.TransformationTriggerer, (EditorTileTransformationTriggerers as IEnumerable<EditorTileModifier>).ToList());

        ModifierCountByCategories.Add(EditorTileModifierCategory.Background, EditorTileBackgrounds.Count);
        ModifierCountByCategories.Add(EditorTileModifierCategory.Attribute, EditorTileAttributes.Count);
        ModifierCountByCategories.Add(EditorTileModifierCategory.TransformationTriggerer, EditorTileTransformationTriggerers.Count);
        
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

    //public List<EditorTileModifier> GetModifiersForCategory(EditorTileModifierCategory category)
    //{
    //    switch (category)
    //    {
    //        case EditorTileModifierCategory.Attribute:
    //            return EditorTileAttributes;
    //            break;
    //        case EditorTileModifierCategory.Background:
    //            break;
    //        case EditorTileModifierCategory.TransformationTriggerer:
    //            break;
    //        default:
    //            break;
    //    }
    //}
}
