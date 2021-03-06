using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorMazeLevelEntryName : MonoBehaviour
{
    [SerializeField] private Text _mazeEntryNameText;
    public MazeLevelEntry MazeLevelEntry { get; private set; }

    private void Awake()
    {
        Guard.CheckIsNull(_mazeEntryNameText, "_mazeEntryNameText", gameObject);
    }

    public void SetText(string text)
    {
        _mazeEntryNameText.text = text;
    }

    public void SetMazeLevelEntry(MazeLevelEntry mazeLevelEntry)
    {
        MazeLevelEntry = mazeLevelEntry;
        SetText(mazeLevelEntry.MazeLevelName);
    }

    public void Destroy()
    {
        MazeLevelEntry = null;
        Destroy(gameObject);
    }
}
