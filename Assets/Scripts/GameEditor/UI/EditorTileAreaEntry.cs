using UnityEngine;
using UnityEngine.UI;

public class EditorTileAreaEntry : MonoBehaviour
{
    [SerializeField] private Text _tileAreaEntryNameText;
    [SerializeField] private GameObject _textAreaEntryNameGO;
    [SerializeField] private Image _textAreaEntrySelectionImage;

    [SerializeField] private InputField _tileAreaEntryInputField;
    
    [SerializeField] private GameObject _saveNewNameButton;
    [SerializeField] private GameObject _editNameButton;

    private TileArea _tileArea;

    private bool _isSelected = false;

    public void Awake()
    {
        Guard.CheckIsNull(_tileAreaEntryNameText, "_tileAreaEntryNameText");
        Guard.CheckIsNull(_textAreaEntryNameGO, "_textAreaEntryNameGO");
        Guard.CheckIsNull(_textAreaEntrySelectionImage, "_textAreaEntrySelectionImage");
        
        Guard.CheckIsNull(_tileAreaEntryInputField, "_tileAreaEntryInputField");
        Guard.CheckIsNull(_saveNewNameButton, "_saveNewNameButton");
        Guard.CheckIsNull(_editNameButton, "_editNameButton");

        _tileAreaEntryInputField.gameObject.SetActive(false);
        _textAreaEntryNameGO.SetActive(true);
        _saveNewNameButton.SetActive(false);
        _editNameButton.SetActive(true);
    }

    public EditorTileAreaEntry WithTileAreaComponent()
    {
        _tileArea = new TileArea("New area");
        GameManager.Instance.CurrentEditorLevel.TileAreas.Add(_tileArea);
        return this;
    }

    public void EditName()
    {
        _tileAreaEntryInputField.text = _tileAreaEntryNameText.text;

        _tileAreaEntryInputField.gameObject.SetActive(true);
        _textAreaEntryNameGO.SetActive(false);
        _saveNewNameButton.SetActive(true);
        _editNameButton.SetActive(false);

        _tileAreaEntryInputField.ActivateInputField();
    }

    public void SetName(string name)
    {
        _tileAreaEntryNameText.text = name;
    }

    public void SaveNewName()
    {
        SetName(_tileAreaEntryInputField.text);
        _tileAreaEntryInputField.gameObject.SetActive(false);
        _textAreaEntryNameGO.SetActive(true);
        _saveNewNameButton.SetActive(false);
        _editNameButton.SetActive(true);

        _tileArea.UpdateName(_tileAreaEntryNameText.text);
    }

    public void Delete()
    {
        TileAreaActionHandler.Instance.DeselectTileAreaEntry(this);

        TileAreaActionHandler.Instance.TileAreaEntries.Remove(this);

        GameManager.Instance.CurrentEditorLevel.TileAreas.Remove(_tileArea);

        GameObject.Destroy(gameObject);
        GameObject.Destroy(this);
    }

    public void Select()
    {
        if (_isSelected) return;

        _isSelected = true;
        _textAreaEntrySelectionImage.enabled = true;

        TileAreaActionHandler.Instance.SelectTileAreaEntry(this);
    }

    public void Deselect()
    {
        if (!_isSelected) return;

        _isSelected = false;
        _textAreaEntrySelectionImage.enabled = false;

        TileAreaActionHandler.Instance.DeselectTileAreaEntry(this);
    }
}
