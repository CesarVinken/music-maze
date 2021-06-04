using UnityEngine;
using UnityEngine.UI;

public class EditorGridModificationPanel : MonoBehaviour, IEditorModificationPanel
{
    [SerializeField] protected InputField _widthInputField;
    [SerializeField] protected InputField _heightInputField;

    protected int _gridWidth = 8;
    protected int _gridHeight = 8;

    public void Awake()
    {
        Guard.CheckIsNull(_heightInputField, "HeightInputField", gameObject);
        Guard.CheckIsNull(_widthInputField, "WidthInputField", gameObject);
    }

    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void SetWidth(string input)
    {
        if (!ValidateNumericInput(input))
        {
            _widthInputField.text = _gridWidth.ToString();
            return;
        }

        _gridWidth = int.Parse(input);
    }

    public void SetHeight(string input)
    {
        if (!ValidateNumericInput(input))
        {
            _heightInputField.text = _gridHeight.ToString();
            return;
        }

        _gridHeight = int.Parse(input);
    }

    public bool ValidateNumericInput(string input)
    {
        if (int.TryParse(input, out int result)) return true;

        Logger.Warning("Could not parse the input '{0}'. Make sure to only give numeric values.", input);
        return false;
    }
}
