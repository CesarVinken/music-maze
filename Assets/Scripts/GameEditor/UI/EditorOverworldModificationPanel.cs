using UnityEngine;
using UnityEngine.UI;

public class EditorOverworldModificationPanel : EditorGridModicationPanel
{
    [SerializeField] private InputField _overworldNameInputField;

    private string _overworldName = "";

    public new void Awake()
    {
        base.Awake();

        Guard.CheckIsNull(_overworldNameInputField, "_overworldNameInputField", gameObject);
    }

    public void SaveOverworld()
    {
        Logger.Log("Save overworld");
    }

    public void LoadOverworld()
    {
        Logger.Log("Load overworld");
    }

    public void SetOverworldName(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            _overworldNameInputField.text = "";
            return;
        }

        _overworldName = input;
    }
}
