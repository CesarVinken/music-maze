using UnityEngine;
using UnityEngine.UI;

public class MazeLevelInvitation : MonoBehaviour
{
    public static MazeLevelInvitation Instance;

    [SerializeField] private Text _infoText;

    public void Awake()
    {
        Instance = this;
    }

    public void Show(string playerName, string mazeName)
    {
        SetInfoText(playerName, mazeName);
    }

    private void SetInfoText(string playerName, string mazeName)
    {
        _infoText.text = $"{playerName} wants to enter the maze {mazeName}. Do you accept this invitation?";
    }

    public void Accept()
    {
        gameObject.SetActive(false);
    }

    public void Reject()
    {
        gameObject.SetActive(false);
    }
}
