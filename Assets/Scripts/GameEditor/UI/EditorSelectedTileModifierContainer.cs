using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class EditorSelectedTileModifierContainer : MonoBehaviour
{
    protected EditorTileAttributeSelector _editorTileAttributeSelector;
    protected EditorTileBackgroundSelector _editorTileBackgroundSelector;
    protected EditorTileTransformationTriggererSelector _editorTileTransformationTriggererSelector;
    protected EditorTileAreaModifierSelector _editorTileAreaModifierSelector;

    public List<EditorTileAttributeModifier> EditorTileAttributes = new List<EditorTileAttributeModifier>();
    public List<EditorTileBackgroundModifier> EditorTileBackgrounds = new List<EditorTileBackgroundModifier>();
    public List<EditorTileTransformationModifier> EditorTileTransformationTriggerers = new List<EditorTileTransformationModifier>();
    public List<EditorTileAreaModifier> EditorTileAreaModifiers = new List<EditorTileAreaModifier>();

    public List<EditorTileModifierCategory> UsedTileModifierCategories = new List<EditorTileModifierCategory>();

    public Dictionary<EditorTileModifierCategory, List<IEditorTileModifier>> CurrentlyAvailableTileModifiers = new Dictionary<EditorTileModifierCategory, List<IEditorTileModifier>>();
    
    public GameObject SelectedModifierLabelGO;
    public GameObject SelectedModifierSpriteGO;

    public Text SelectedModifierLabel;
    public Image SelectedModifierSprite;

    public abstract void SelectPreviousTileModifier();
    public abstract void SelectNextTileModifier();

    public void SetSelectedTileModifierCategory(EditorTileModifierCategory editorMazeTileModifierCategory)
    {
        if (EditorManager.SelectedTileModifierCategory == EditorTileModifierCategory.TransformationTriggerer)
        {
            EditorTileTransformationModifier editorMazeTileTransformationTriggerer = EditorTileTransformationTriggerers[EditorManager.SelectedTileTransformationTriggererIndex];
            editorMazeTileTransformationTriggerer.UnsetSelectedTile();
        }

        EditorManager.SelectedTileModifierCategory = editorMazeTileModifierCategory;
        EditorSelectedTileModifierCategoryContainer.Instance.SetCategoryLabel(EditorManager.SelectedTileModifierCategory);
    }

    public void SetSelectedTileModifier(int modifierIndex)
    {
        if (EditorManager.SelectedTileModifierCategory == EditorTileModifierCategory.Attribute)
        {
            _editorTileAttributeSelector.SetSelectedModifier(modifierIndex);
        }
        else if (EditorManager.SelectedTileModifierCategory == EditorTileModifierCategory.Background)
        {
            _editorTileBackgroundSelector.SetSelectedModifier(modifierIndex);
        }
        else if (EditorManager.SelectedTileModifierCategory == EditorTileModifierCategory.TransformationTriggerer)
        {
            _editorTileTransformationTriggererSelector.SetSelectedModifier(modifierIndex);
        }
        else if (EditorManager.SelectedTileModifierCategory == EditorTileModifierCategory.Area)
        {
            _editorTileAreaModifierSelector.SetSelectedModifier(modifierIndex);
        }
        else
        {
            Logger.Error("Unknown modifier type");
        }
    }

    public void Reset()
    {
        EditorTileAttributes.Clear();
        EditorTileBackgrounds.Clear();
        EditorTileTransformationTriggerers.Clear();
        EditorTileAreaModifiers.Clear();

        UsedTileModifierCategories.Clear();
    }

    public void SetCurrentlyAvailableModifierCategories(EditorTileMainModifierCategory mainTileModifierCategory)
    {
        CurrentlyAvailableTileModifiers.Clear();

        if (mainTileModifierCategory is EditorMazeTileGroundModifierCategory || mainTileModifierCategory is EditorOverworldTileGroundModifierCategory)
        {
            Logger.Log("SetCurrentlyAvailableModifiers for ground");
            SetCurrentlyAvailableGroundModifiers();
        }
        else if(mainTileModifierCategory is EditorMazeTileWaterModifierCategory || mainTileModifierCategory is EditorOverworldTileWaterModifierCategory)
        {
            Logger.Log("SetCurrentlyAvailableModifiers for water");
            SetCurrentlyAvailableWaterModifiers();
        }
        else if (mainTileModifierCategory is EditorMazeTileAreaModifierCategory || mainTileModifierCategory is EditorOverworldTileAreaModifierCategory)
        {
            Logger.Log("SetCurrentlyAvailableModifiers for areas");
            SetCurrentlyAvailableAreaModifiers();
        }
        else
        {
            Logger.Error($"Unknown modifier category {mainTileModifierCategory}");
        }
    }

    private void SetCurrentlyAvailableWaterModifiers()
    {
        List<IEditorTileModifier> currentlyAvailableBackgrounds = new List<IEditorTileModifier>();
        for (int i = 0; i < EditorTileBackgrounds.Count; i++)
        {
            if (EditorTileBackgrounds[i] is IWaterMaterialModifier)
            {
                currentlyAvailableBackgrounds.Add(EditorTileBackgrounds[i]);
            }
        }
        CurrentlyAvailableTileModifiers.Add(EditorTileModifierCategory.Background, currentlyAvailableBackgrounds);

        List<IEditorTileModifier> currentlyAvailableAttributes = new List<IEditorTileModifier>();
        for (int i = 0; i < EditorTileAttributes.Count; i++)
        {
            if (EditorTileAttributes[i] is IWaterMaterialModifier)
            {
                currentlyAvailableAttributes.Add(EditorTileAttributes[i]);
            }
        }
        CurrentlyAvailableTileModifiers.Add(EditorTileModifierCategory.Attribute, currentlyAvailableAttributes);

        List<IEditorTileModifier> currentlyAvailableTransformationTriggers = new List<IEditorTileModifier>();
        for (int i = 0; i < EditorTileTransformationTriggerers.Count; i++)
        {
            if (EditorTileTransformationTriggerers[i] is IWaterMaterialModifier)
            {
                currentlyAvailableTransformationTriggers.Add(EditorTileTransformationTriggerers[i]);
            }
        }
        CurrentlyAvailableTileModifiers.Add(EditorTileModifierCategory.TransformationTriggerer, currentlyAvailableTransformationTriggers);

        // initial value for water: 
        EditorCanvasUI.Instance.SelectedTileModifierContainer.SetSelectedTileModifierCategory(EditorTileModifierCategory.Background);
        EditorCanvasUI.Instance.SelectedTileModifierContainer.SetSelectedTileModifier(0);
    }

    private void SetCurrentlyAvailableGroundModifiers()
    {
        List<IEditorTileModifier> currentlyAvailableBackgrounds = new List<IEditorTileModifier>();
        for (int i = 0; i < EditorTileBackgrounds.Count; i++)
        {
            if (EditorTileBackgrounds[i] is IGroundMaterialModifier)
            {
                currentlyAvailableBackgrounds.Add(EditorTileBackgrounds[i]);
            }
        }
        CurrentlyAvailableTileModifiers.Add(EditorTileModifierCategory.Background, currentlyAvailableBackgrounds);

        List<IEditorTileModifier> currentlyAvailableAttributes = new List<IEditorTileModifier>();
        for (int i = 0; i < EditorTileAttributes.Count; i++)
        {
            if (EditorTileAttributes[i] is IGroundMaterialModifier)
            {
                currentlyAvailableAttributes.Add(EditorTileAttributes[i]);
            }
        }
        CurrentlyAvailableTileModifiers.Add(EditorTileModifierCategory.Attribute, currentlyAvailableAttributes);

        List<IEditorTileModifier> currentlyAvailableTransformationTriggers = new List<IEditorTileModifier>();
        for (int i = 0; i < EditorTileTransformationTriggerers.Count; i++)
        {
            if (EditorTileTransformationTriggerers[i] is IGroundMaterialModifier)
            {
                currentlyAvailableTransformationTriggers.Add(EditorTileTransformationTriggerers[i]);
            }
        }
        CurrentlyAvailableTileModifiers.Add(EditorTileModifierCategory.TransformationTriggerer, currentlyAvailableTransformationTriggers);

        // initial value for groud:  Background -> path
        EditorCanvasUI.Instance.SelectedTileModifierContainer.SetSelectedTileModifierCategory(EditorTileModifierCategory.Background);
        EditorCanvasUI.Instance.SelectedTileModifierContainer.SetSelectedTileModifier(0);
    }

    private void SetCurrentlyAvailableAreaModifiers()
    {
        List<IEditorTileModifier> currentlyAvailableAreaModifiers = new List<IEditorTileModifier>();
        for (int i = 0; i < EditorTileAreaModifiers.Count; i++)
        {       
            currentlyAvailableAreaModifiers.Add(EditorTileAreaModifiers[i]);       
        }
        CurrentlyAvailableTileModifiers.Add(EditorTileModifierCategory.Area, currentlyAvailableAreaModifiers);

        // initial value for area: 
        EditorCanvasUI.Instance.SelectedTileModifierContainer.SetSelectedTileModifierCategory(EditorTileModifierCategory.Area);
        EditorCanvasUI.Instance.SelectedTileModifierContainer.SetSelectedTileModifier(0);
    }
}
