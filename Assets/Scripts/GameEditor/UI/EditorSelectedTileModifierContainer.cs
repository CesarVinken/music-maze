using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorSelectedTileModifierContainer : MonoBehaviour
{
    public static EditorSelectedTileModifierContainer Instance;

    public GameObject SelectedModifierLabelGO;
    public GameObject SelectedModifierSpriteGO;

    public Text SelectedModifierLabel;
    public Image SelectedModifierSprite;

    public List<IEditorMazeTileAttribute> EditorMazeTileAttributes = new List<IEditorMazeTileAttribute>();
    public List<IEditorMazeTileBackground> EditorMazeTileBackgrounds = new List<IEditorMazeTileBackground>();
    public List<IEditorMazeTileTransformationTriggerer> EditorMazeTileTransformationTriggerers = new List<IEditorMazeTileTransformationTriggerer>();

    public Dictionary<EditorMazeTileModifierType, EditorMazeTileModifierSelector> ModifierSelectorsByType = new Dictionary<EditorMazeTileModifierType, EditorMazeTileModifierSelector>();

    private EditorMazeTileAttributeSelector _editorMazeTileAttributeSelector;
    private EditorMazeTileBackgroundSelector _editorMazeTileBackgroundSelector;
    private EditorMazeTileTransformationTriggererSelector _editorMazeTileTransformationTriggererSelector;

    void Awake()
    {
        Guard.CheckIsNull(SelectedModifierLabelGO, "SelectedModifierLabelGO", gameObject);
        Guard.CheckIsNull(SelectedModifierSpriteGO, "SelectedModifierSpriteGO", gameObject);
        Guard.CheckIsNull(SelectedModifierLabel, "SelectedModifierLabel", gameObject);
        Guard.CheckIsNull(SelectedModifierSprite, "SelectedModifierSprite", gameObject);

        Instance = this;

        _editorMazeTileAttributeSelector = new EditorMazeTileAttributeSelector(this);
        _editorMazeTileBackgroundSelector = new EditorMazeTileBackgroundSelector(this);
        _editorMazeTileTransformationTriggererSelector = new EditorMazeTileTransformationTriggererSelector(this);

        EditorMazeTileAttributes.Clear();

        EditorMazeTileAttributes.Add(new EditorObstacleTileAttribute());
        EditorMazeTileAttributes.Add(new EditorPlayerExitTileAttribute());
        EditorMazeTileAttributes.Add(new EditorPlayerSpawnpointTileAttribute());
        EditorMazeTileAttributes.Add(new EditorPlayerOnlyTileAttribute());
        EditorMazeTileAttributes.Add(new EditorEnemySpawnpointTileAttribute());

        EditorManager.SelectedMazeTileAttributeModifierIndex = 0;

        EditorMazeTileBackgrounds.Clear();

        EditorMazeTileBackgrounds.Add(new EditorMazeTilePath());
        EditorManager.SelectedMazeTileBackgroundModifierIndex = 0;

        EditorMazeTileTransformationTriggerers.Clear();

        EditorMazeTileTransformationTriggerers.Add(new EditorMazeTileTransformationTriggerer());
        EditorManager.SelectedMazeTileTransformationTriggererIndex = 0;

        ModifierSelectorsByType.Add(EditorMazeTileModifierType.Attribute, _editorMazeTileAttributeSelector);
        ModifierSelectorsByType.Add(EditorMazeTileModifierType.Background, _editorMazeTileBackgroundSelector);
    }

    public void Start()
    {
        SetSelectedMazeTileModifierCategory(EditorMazeTileModifierType.Attribute);
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

    public void SelectPreviousTileModifier()
    {
        if (EditorManager.SelectedMazeTileModifierCategory == EditorMazeTileModifierType.Background)
        {
            _editorMazeTileBackgroundSelector.SwitchSelectedModifier(-1);
        }
        else if (EditorManager.SelectedMazeTileModifierCategory == EditorMazeTileModifierType.Attribute)
        {
            _editorMazeTileAttributeSelector.SwitchSelectedModifier(-1);
        }
        else if (EditorManager.SelectedMazeTileModifierCategory == EditorMazeTileModifierType.TransformationTriggerer)
        {
            _editorMazeTileTransformationTriggererSelector.SwitchSelectedModifier(-1);
        }
        else
        {
            // Not known type
            Logger.Error("Unknown maze tile modifier type");
        }
    }

    public void SelectNextTileModifier()
    {
        if (EditorManager.SelectedMazeTileModifierCategory == EditorMazeTileModifierType.Background)
        {
            _editorMazeTileBackgroundSelector.SwitchSelectedModifier(1);
        }
        else if (EditorManager.SelectedMazeTileModifierCategory == EditorMazeTileModifierType.Attribute)
        {
            _editorMazeTileAttributeSelector.SwitchSelectedModifier(1);
        }
        else if (EditorManager.SelectedMazeTileModifierCategory == EditorMazeTileModifierType.TransformationTriggerer)
        {
            _editorMazeTileTransformationTriggererSelector.SwitchSelectedModifier(1);
        }
        else
        {
            // Not known type
            Logger.Error("Unknown maze tile modifier type");
        }
    }

    public void SetSelectedMazeTileModifierCategory(EditorMazeTileModifierType editorMazeTileModifierCategory)
    {   
        if (EditorManager.SelectedMazeTileModifierCategory == EditorMazeTileModifierType.TransformationTriggerer)
        {
            EditorMazeTileTransformationTriggerer editorMazeTileTransformationTriggerer = EditorMazeTileTransformationTriggerers[EditorManager.SelectedMazeTileTransformationTriggererIndex] as EditorMazeTileTransformationTriggerer;
            editorMazeTileTransformationTriggerer.UnsetSelectedTile();
        }

        EditorManager.SelectedMazeTileModifierCategory = editorMazeTileModifierCategory;
        EditorSelectedTileModifierCategoryContainer.Instance.SetCategoryLabel(EditorManager.SelectedMazeTileModifierCategory);
    }

    public void SetSelectedMazeTileModifier(int modifierIndex)
    {
        if (EditorManager.SelectedMazeTileModifierCategory == EditorMazeTileModifierType.Attribute)
        {
            _editorMazeTileAttributeSelector.SetSelectedModifier(modifierIndex);
        }
        else if (EditorManager.SelectedMazeTileModifierCategory == EditorMazeTileModifierType.Background)
        {
            _editorMazeTileBackgroundSelector.SetSelectedModifier(modifierIndex);
        }
        else if (EditorManager.SelectedMazeTileModifierCategory == EditorMazeTileModifierType.TransformationTriggerer)
        {
            _editorMazeTileTransformationTriggererSelector.SetSelectedModifier(modifierIndex);
        }
        else
        {
            Logger.Error("Unknown modifier type");
        }
    }
}
