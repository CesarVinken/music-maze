using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class EditorSelectedTileModifierContainer : MonoBehaviour
{
    public List<EditorTileAttributeModifier> EditorTileAttributes = new List<EditorTileAttributeModifier>();
    public List<EditorTileBackgroundModifier> EditorTileBackgrounds = new List<EditorTileBackgroundModifier>();
    public List<EditorTileTransformationModifier> EditorTileTransformationTriggerers = new List<EditorTileTransformationModifier>();

    public List<EditorTileModifierCategory> UsedTileModifierCategories = new List<EditorTileModifierCategory>();
    public Dictionary<EditorTileModifierCategory, List<EditorTileModifier>> ModifiersByCategories = new Dictionary<EditorTileModifierCategory, List<EditorTileModifier>>();

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
