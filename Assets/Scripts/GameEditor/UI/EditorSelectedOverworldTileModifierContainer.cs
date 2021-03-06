﻿using UnityEngine;

public class EditorSelectedOverworldTileModifierContainer : EditorSelectedTileModifierContainer
{
    void Awake()
    {
        Guard.CheckIsNull(SelectedModifierLabelGO, "SelectedModifierLabelGO", gameObject);
        Guard.CheckIsNull(SelectedModifierSpriteGO, "SelectedModifierSpriteGO", gameObject);
        Guard.CheckIsNull(SelectedModifierLabel, "SelectedModifierLabel", gameObject);
        Guard.CheckIsNull(SelectedModifierSprite, "SelectedModifierSprite", gameObject);

        EditorCanvasUI.Instance.SelectedTileModifierContainer = this;

        _editorTileAttributeSelector = new EditorTileAttributeSelector(this);
        _editorTileBackgroundSelector = new EditorTileBackgroundSelector(this);
        _editorTileTransformationTriggererSelector = new EditorTileTransformationTriggererSelector(this);

        EditorTileAttributes.Clear();

        //EditorTileAttributes.Add(new EditorObstacleTileAttribute());
        //EditorTileAttributes.Add(new EditorPlayerExitTileAttribute());
        EditorTileAttributes.Add(new EditorPlayerSpawnpointOverworldTileAttribute());
        EditorTileAttributes.Add(new EditorOverworldMazeLevelEntryTileAttribute());
        //EditorTileAttributes.Add(new EditorPlayerOnlyTileAttribute());
        //EditorTileAttributes.Add(new EditorEnemySpawnpointTileAttribute());

        EditorManager.SelectedTileAttributeModifierIndex = 0;

        EditorTileBackgrounds.Clear();

        EditorTileBackgrounds.Add(new EditorOverworldTilePath());
        EditorManager.SelectedTileBackgroundModifierIndex = 0;

        EditorTileTransformationTriggerers.Clear();

        //EditorTileTransformationTriggerers.Add(new EditorMazeTileTransformationTriggerer());
        EditorManager.SelectedTileTransformationTriggererIndex = 0;

        UsedTileModifierCategories.Add(EditorTileModifierCategory.Background);
        UsedTileModifierCategories.Add(EditorTileModifierCategory.Attribute);

        ModifierCountByCategories.Add(EditorTileModifierCategory.Background, EditorTileBackgrounds.Count);
        ModifierCountByCategories.Add(EditorTileModifierCategory.Attribute, EditorTileAttributes.Count);

        SetInitialModifierValue();
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
        else
        {
            // Not known type
            Logger.Error("Unknown tile modifier type");
        }
    }

    private void SetInitialModifierValue()
    {
        SetSelectedTileModifierCategory(EditorTileModifierCategory.Background);
        SetSelectedTileModifier(0);//Set selected modifier to Background -> Path 
    }
}
