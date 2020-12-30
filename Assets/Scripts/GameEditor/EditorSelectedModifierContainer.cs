using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorSelectedModifierContainer : MonoBehaviour
{
    public static EditorSelectedModifierContainer Instance;

    public GameObject SelectedModifierLabelGO;
    public GameObject SelectedModifierSpriteGO;

    public Text SelectedModifierLabel;
    public Image SelectedModifierSprite;

    public List<IEditorMazeTileAttribute> EditorMazeTileAttributes = new List<IEditorMazeTileAttribute>();
    public List<IEditorMazeTileBackground> EditorMazeTileBackgrounds = new List<IEditorMazeTileBackground>();

    public Dictionary<EditorMazeTileModifierType, EditorMazeTileModifierSelector> ModifierSelectorsByType = new Dictionary<EditorMazeTileModifierType, EditorMazeTileModifierSelector>();

    private EditorMazeTileAttributeSelector _editorMazeTileAttributeSelector;
    private EditorMazeTileBackgroundSelector _editorMazeTileBackgroundSelector;

    void Awake()
    {
        Guard.CheckIsNull(SelectedModifierLabelGO, "SelectedModifierLabelGO", gameObject);
        Guard.CheckIsNull(SelectedModifierSpriteGO, "SelectedModifierSpriteGO", gameObject);
        Guard.CheckIsNull(SelectedModifierLabel, "SelectedModifierLabel", gameObject);
        Guard.CheckIsNull(SelectedModifierSprite, "SelectedModifierSprite", gameObject);

        Instance = this;

        _editorMazeTileAttributeSelector = new EditorMazeTileAttributeSelector(this);
        _editorMazeTileBackgroundSelector = new EditorMazeTileBackgroundSelector(this);

        EditorMazeTileAttributes.Clear();

        EditorMazeTileAttributes.Add(new EditorObstacleTileAttribute());
        EditorMazeTileAttributes.Add(new EditorPlayerExitTileAttribute());
        EditorMazeTileAttributes.Add(new EditorPlayerSpawnpointTileAttribute());
        EditorMazeTileAttributes.Add(new EditorPlayerOnlyTileAttribute());
        EditorMazeTileAttributes.Add(new EditorEnemySpawnpointTileAttribute());

        _editorMazeTileAttributeSelector.SetSelectedModifier(0);

        EditorMazeTileBackgrounds.Clear();

        EditorMazeTileBackgrounds.Add(new EditorMazeTilePath());

        _editorMazeTileBackgroundSelector.SetSelectedModifier(0);

        ModifierSelectorsByType.Add(EditorMazeTileModifierType.Attribute, _editorMazeTileAttributeSelector);
        ModifierSelectorsByType.Add(EditorMazeTileModifierType.Background, _editorMazeTileBackgroundSelector);

        SetSelectedMazeTileModifierType(EditorMazeTileModifierType.Attribute);
        SetSelectedMazeTileModifier(0);//Set selected modifier to Background -> Path
    }

    private void Update()
    {
        if (!EditorManager.InEditor) return;

        if (Input.GetKeyDown(KeyCode.PageDown))
        {
            if (EditorManager.SelectedMazeTileModifierType == EditorMazeTileModifierType.Background)
            {
                _editorMazeTileBackgroundSelector.SwitchSelectedModifier(1);
            }
            else if(EditorManager.SelectedMazeTileModifierType == EditorMazeTileModifierType.Attribute)
            {
                _editorMazeTileAttributeSelector.SwitchSelectedModifier(1);
            }
            else
            {
                // Not known type
                Logger.Error("Unknown maze tile modifier type");
            }
        }
        else if (Input.GetKeyDown(KeyCode.PageUp))
        {
            if (EditorManager.SelectedMazeTileModifierType == EditorMazeTileModifierType.Background)
            {
                _editorMazeTileBackgroundSelector.SwitchSelectedModifier(-1);
            }
            else if (EditorManager.SelectedMazeTileModifierType == EditorMazeTileModifierType.Attribute)
            {
                _editorMazeTileAttributeSelector.SwitchSelectedModifier(-1);
            }
            else
            {
                // Not known type
                Logger.Error("Unknown maze tile modifier type");
            }
        }
    }

    // background or attribute
    public void SetSelectedMazeTileModifierType(EditorMazeTileModifierType editorMazeTileModifierType)
    {
        EditorManager.SelectedMazeTileModifierType = editorMazeTileModifierType;
    }

    public void SetSelectedMazeTileModifier(int modifierIndex)
    {
        if (EditorManager.SelectedMazeTileModifierType == EditorMazeTileModifierType.Attribute)
        {
            _editorMazeTileAttributeSelector.SetSelectedModifier(modifierIndex);
        }
        else if (EditorManager.SelectedMazeTileModifierType == EditorMazeTileModifierType.Background)
        {
            _editorMazeTileBackgroundSelector.SetSelectedModifier(modifierIndex);
        }
        else
        {
            Logger.Error("Unknown modifier type");
        }
    }
}
