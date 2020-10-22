﻿using UnityEngine;

public class EditorContainer : MonoBehaviour
{
    public static EditorContainer Instance;

    public GameObject EditorModeStatusTextGO;
    public GameObject EditorUIGO;

    public void Awake()
    {
        Instance = this;

        Guard.CheckIsNull(EditorModeStatusTextGO, "EditorModeStatusTextGO");
        Guard.CheckIsNull(EditorUIGO, "EditorUIGO");
    }

    public void InitialiseEditor()
    {
        EditorModeStatusTextGO.SetActive(true);
        EditorUIGO.SetActive(true);
    }

    public void CloseEditor()
    {
        EditorModeStatusTextGO.SetActive(false);
        EditorUIGO.SetActive(false);
    }
}
