﻿using UnityEngine;

public class EditorTileModificationPanel : MonoBehaviour, IEditorModificationPanel
{
    public static EditorTileModificationPanel Instance;

    public Transform TileModifierActionsContainer;

    public void Awake()
    {
        Instance = this;

        Guard.CheckIsNull(TileModifierActionsContainer, "TileModifierActionsContainer");
    }

    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void DestroyModifierActions()
    {
        Logger.Log("Destroy actions for triggerer");
        foreach (Transform action in TileModifierActionsContainer)
        {
            GameObject.Destroy(action.gameObject);
        }
    }

}
