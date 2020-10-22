using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EditorManager
{
    public static bool InEditor = false;

    public static void ToggleEditorMode()
    {
        if (InEditor)
        {
            OpenEditor();
            return;
        }

        CloseEditor();
    }

    public static void OpenEditor()
    {
        InEditor = true;
    }

    public static void CloseEditor()
    {
        InEditor = false;
    }
}
