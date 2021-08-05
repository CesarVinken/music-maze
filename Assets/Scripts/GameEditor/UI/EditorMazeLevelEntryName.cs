using UnityEngine;
using UnityEngine.UI;

public class EditorMazeLevelEntryName : MonoBehaviour
{
    [SerializeField] private Text _mazeEntryNameText;
    private Vector3 _worldPosition;

    public MazeLevelEntry MazeLevelEntry { get; private set; }

    private void Awake()
    {
        Guard.CheckIsNull(_mazeEntryNameText, "_mazeEntryNameText", gameObject);
    }

    public void Update()
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(_worldPosition);
        transform.position = screenPos;
    }

    public void SetWorldPosition(Vector3 worldPosition)
    {
        _worldPosition = worldPosition;
    }

    public void SetText(string text)
    {
        _mazeEntryNameText.text = text;
    }

    public void SetMazeLevelEntry(MazeLevelEntry mazeLevelEntry)
    {
        Logger.Log($"Set name to {mazeLevelEntry.MazeLevelName}");
        MazeLevelEntry = mazeLevelEntry;
        SetText(mazeLevelEntry.MazeLevelName);
    }

    public void Destroy()
    {
        MazeLevelEntry = null;
        Destroy(gameObject);
    }
}
