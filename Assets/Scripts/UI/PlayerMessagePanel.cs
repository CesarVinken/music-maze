using UnityEngine;
using UnityEngine.UI;

public class PlayerMessagePanel : MonoBehaviour
{
    public static PlayerMessagePanel Instance;

    [SerializeField] private Text _messageText;

    public void ShowMessage( string message)
    {
        Instance = this;
        
        _messageText.text = message;
        gameObject.SetActive(true);
    }

    public void CloseMessagePanel()
    {
        Instance = null;
        Destroy(gameObject);
    }
}
