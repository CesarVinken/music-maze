using UnityEngine;
using UnityEngine.UI;

public class PlayerNameInputPanel : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private InputField _playerNameInputField = null;
    [SerializeField] private Button _continueButton = null;

    public static string DisplayName { get; private set; }
    private const string _playerPrefsNameKey = "PlayerName";

    private void Start()
    {
        SetupInputField();
    }

    private void SetupInputField()
    {
        if (!PlayerPrefs.HasKey(_playerPrefsNameKey)) { return; }

        string defaultName = PlayerPrefs.GetString(_playerPrefsNameKey);

        _playerNameInputField.text = defaultName;

        SetPlayerName(defaultName);
    }

    public void SetPlayerName(string name)
    {
        _continueButton.interactable = !string.IsNullOrEmpty(name);
    }

    public void SavePlayerName()
    {
        DisplayName = _playerNameInputField.text;

        PlayerPrefs.SetString(_playerPrefsNameKey, DisplayName);
    }
}
