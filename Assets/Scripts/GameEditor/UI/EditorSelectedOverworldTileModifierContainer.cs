﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EditorSelectedOverworldTileModifierContainer : EditorSelectedTileModifierContainer
{
    public static EditorSelectedOverworldTileModifierContainer Instance;

    private EditorTileAttributeSelector _editorOverworldTileAttributeSelector;
    private EditorTileBackgroundSelector _editorOverworldTileBackgroundSelector;
    private EditorTileTransformationTriggererSelector _editorOverworldTileTransformationTriggererSelector;

    void Awake()
    {
        Guard.CheckIsNull(SelectedModifierLabelGO, "SelectedModifierLabelGO", gameObject);
        Guard.CheckIsNull(SelectedModifierSpriteGO, "SelectedModifierSpriteGO", gameObject);
        Guard.CheckIsNull(SelectedModifierLabel, "SelectedModifierLabel", gameObject);
        Guard.CheckIsNull(SelectedModifierSprite, "SelectedModifierSprite", gameObject);

        Instance = this;

        _editorOverworldTileAttributeSelector = new EditorTileAttributeSelector(this);
        _editorOverworldTileBackgroundSelector = new EditorTileBackgroundSelector(this);
        _editorOverworldTileTransformationTriggererSelector = new EditorTileTransformationTriggererSelector(this);

        EditorTileAttributes.Clear();

        //EditorTileAttributes.Add(new EditorObstacleTileAttribute());
        //EditorTileAttributes.Add(new EditorPlayerExitTileAttribute());
        //EditorTileAttributes.Add(new EditorPlayerSpawnpointTileAttribute());
        //EditorTileAttributes.Add(new EditorPlayerOnlyTileAttribute());
        //EditorTileAttributes.Add(new EditorEnemySpawnpointTileAttribute());

        EditorManager.SelectedTileAttributeModifierIndex = 0;

        EditorTileBackgrounds.Clear();

        EditorTileBackgrounds.Add(new EditorOverworldTilePath() as IEditorTileBackground<Tile>);
        EditorManager.SelectedTileBackgroundModifierIndex = 0;

        EditorTileTransformationTriggerers.Clear();

        //EditorTileTransformationTriggerers.Add(new EditorMazeTileTransformationTriggerer());
        EditorManager.SelectedTileTransformationTriggererIndex = 0;

        UsedTileModifierCategories.Add(EditorTileModifierCategory.Background);
        UsedTileModifierCategories.Add(EditorTileModifierCategory.Attribute);


        ModifiersByCategories.Add(EditorTileModifierCategory.Background, (EditorTileBackgrounds as IEnumerable<IEditorTileModifierType>).ToList());
        ModifiersByCategories.Add(EditorTileModifierCategory.Attribute, (EditorTileAttributes as IEnumerable<IEditorTileModifierType>).ToList());

        //ModifierSelectorsByType.Add(EditorTileModifierCategory.Attribute, _editorOverworldTileAttributeSelector);
        //ModifierSelectorsByType.Add(EditorTileModifierCategory.Background, _editorOverworldTileBackgroundSelector);
        //ModifierSelectorsByType.Add(EditorTileModifierCategory.TransformationTriggerer, _editorOverworldTileTransformationTriggererSelector);
    }

    public override void SelectPreviousTileModifier()
    {
        if (EditorManager.SelectedTileModifierCategory == EditorTileModifierCategory.Background)
        {
            _editorOverworldTileBackgroundSelector.SwitchSelectedModifier(-1);
        }
        else if (EditorManager.SelectedTileModifierCategory == EditorTileModifierCategory.Attribute)
        {
            _editorOverworldTileAttributeSelector.SwitchSelectedModifier(-1);
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
            _editorOverworldTileBackgroundSelector.SwitchSelectedModifier(1);
        }
        else if (EditorManager.SelectedTileModifierCategory == EditorTileModifierCategory.Attribute)
        {
            _editorOverworldTileBackgroundSelector.SwitchSelectedModifier(1);
        }
        else
        {
            // Not known type
            Logger.Error("Unknown tile modifier type");
        }
    }
}
