using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class EditorSelectedTileModifierContainer : MonoBehaviour
{
    protected EditorTileMainMaterialSelector _editorTileMainMaterialSelector;
    protected EditorTileAttributeSelector _editorTileAttributeSelector;
    protected EditorTileBackgroundSelector _editorTileBackgroundSelector;
    protected EditorTileTransformationTriggererSelector _editorTileTransformationTriggererSelector;

    public List<EditorTileMainMaterialModifier> EditorTileMainMaterials = new List<EditorTileMainMaterialModifier>();
    public List<EditorTileAttributeModifier> EditorTileAttributes = new List<EditorTileAttributeModifier>();
    public List<EditorTileBackgroundModifier> EditorTileBackgrounds = new List<EditorTileBackgroundModifier>();
    public List<EditorTileTransformationModifier> EditorTileTransformationTriggerers = new List<EditorTileTransformationModifier>();

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
        if (EditorManager.SelectedTileModifierCategory == EditorTileModifierCategory.MainMaterial)
        {
            _editorTileMainMaterialSelector.SetSelectedModifier(modifierIndex);
        }
        else if (EditorManager.SelectedTileModifierCategory == EditorTileModifierCategory.Attribute)
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
        else
        {
            Logger.Error("Unknown modifier type");
        }
    }

    public void Reset()
    {
        EditorTileMainMaterials.Clear();
        EditorTileAttributes.Clear();
        EditorTileBackgrounds.Clear();
        EditorTileTransformationTriggerers.Clear();

        UsedTileModifierCategories.Clear();
    }

    // TODO: make work with other MainMaterials than Ground
    public void SetCurrentlyAvailableModifiers()
    {
        CurrentlyAvailableTileModifiers.Clear();

        List<IEditorTileModifier> currentlyAvailableMainMaterials = new List<IEditorTileModifier>();
        for (int i = 0; i < EditorTileMainMaterials.Count; i++)
        {
            //if(EditorTileMainMaterials[i] is IGroundMaterialModifier) 
            //{
            currentlyAvailableMainMaterials.Add(EditorTileMainMaterials[i]);
            //}
        }
        CurrentlyAvailableTileModifiers.Add(EditorTileModifierCategory.MainMaterial, currentlyAvailableMainMaterials);

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
    }
}
