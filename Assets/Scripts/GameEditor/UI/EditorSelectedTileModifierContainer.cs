﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class EditorSelectedTileModifierContainer : MonoBehaviour
{
    public List<IEditorTileAttribute<Tile>> EditorTileAttributes = new List<IEditorTileAttribute<Tile>>();
    public List<IEditorTileBackground<Tile>> EditorTileBackgrounds = new List<IEditorTileBackground<Tile>>();
    public List<IEditorTileTransformationTriggerer<Tile>> EditorTileTransformationTriggerers = new List<IEditorTileTransformationTriggerer<Tile>>();

    public List<EditorTileModifierCategory> UsedTileModifierCategories = new List<EditorTileModifierCategory>();
    public Dictionary<EditorTileModifierCategory, List<IEditorTileModifierType>> ModifiersByCategories = new Dictionary<EditorTileModifierCategory, List<IEditorTileModifierType>>();

    //public Dictionary<EditorTileModifierCategory, EditorTileModifierSelector> ModifierSelectorsByType = new Dictionary<EditorTileModifierCategory, EditorTileModifierSelector>();

    public GameObject SelectedModifierLabelGO;
    public GameObject SelectedModifierSpriteGO;

    public Text SelectedModifierLabel;
    public Image SelectedModifierSprite;

    public abstract void SelectPreviousTileModifier();
    public abstract void SelectNextTileModifier();

    public abstract void SetSelectedMazeTileModifierCategory(EditorTileModifierCategory editorTileModifierCategory);
    public abstract void SetSelectedMazeTileModifier(int index);

    public void Reset()
    {
        EditorTileAttributes.Clear();
        EditorTileBackgrounds.Clear();
        EditorTileTransformationTriggerers.Clear();

        UsedTileModifierCategories.Clear();
    }
}
