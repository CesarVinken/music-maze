using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorSelectedAttributeContainer : MonoBehaviour
{
    public static EditorSelectedAttributeContainer Instance;

    public GameObject SelectedAttributeLabelGO;
    public GameObject SelectedAttributeSpriteGO;

    public Text SelectedAttributeLabel;
    public Image SelectedAttributeSprite;

    public List<IEditorMazeTileAttribute> EditorMazeTileAttributes = new List<IEditorMazeTileAttribute>();

    void Awake()
    {
        Guard.CheckIsNull(SelectedAttributeLabelGO, "SelectedAttributeLabelGO", gameObject);
        Guard.CheckIsNull(SelectedAttributeSpriteGO, "SelectedAttributeSpriteGO", gameObject);
        Guard.CheckIsNull(SelectedAttributeLabel, "SelectedAttributeLabel", gameObject);
        Guard.CheckIsNull(SelectedAttributeSprite, "SelectedAttributeSprite", gameObject);

        Instance = this;

        EditorMazeTileAttributes.Clear();

        EditorMazeTileAttributes.Add(new EditorObstacleTileAttribute());
        EditorMazeTileAttributes.Add(new EditorPlayerExitTileAttribute());
        //EditorMazeTileAttributes.Add(new EditorMazeTileAttribute(EditorMazeTileAttributeType.EnemySpawnpoint));
        //EditorMazeTileAttributes.Add(new EditorMazeTileAttribute(EditorMazeTileAttributeType.PlayerExit));

        IEditorMazeTileAttribute defaultAttribute = EditorMazeTileAttributes[0];
        SetSelectedAttribute(defaultAttribute);
    }

    private void Update()
    {
        if (!EditorManager.InEditor) return;

        if (Input.GetKeyDown(KeyCode.PageDown))
        {
            SwitchSelectedAttribute(1);
        }
        else if (Input.GetKeyDown(KeyCode.PageUp))
        {
            SwitchSelectedAttribute(-1);
        }
    }

    public void SwitchSelectedAttribute(int newValue)
    {
        int selectedAttributeIndex = EditorMazeTileAttributes.FindIndex(a => a.AttributeType == EditorManager.SelectedMazeTileAttributeType);
        int newIndex = selectedAttributeIndex + newValue;

        if (newIndex < 0)
            newIndex = EditorMazeTileAttributes.Count - 1;
        if (newIndex >= EditorMazeTileAttributes.Count)
            newIndex = 0;

        IEditorMazeTileAttribute attribute = EditorMazeTileAttributes[newIndex];
        SetSelectedAttribute(attribute);
    }

    public void SetSelectedAttribute(IEditorMazeTileAttribute attribute)
    {
        SelectedAttributeLabel.text = GetSelectedAttributeLabel(attribute.Name);
        SelectedAttributeSprite.sprite = attribute.Sprite;

        EditorManager.SelectedMazeTileAttributeType = attribute.AttributeType;
    }

    private string GetSelectedAttributeLabel(string attributeName)
    {
        return $"Selected: {attributeName}";
    }
}
