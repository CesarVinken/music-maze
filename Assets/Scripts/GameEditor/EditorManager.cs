﻿
public static class EditorManager
{
    private static bool _inEditor = false;
    public static bool InEditor
    {
        get { return _inEditor; }
        set
        {

            _inEditor = value;
        }
    }

    public static EditorTileModifierCategory SelectedTileModifierCategory;
    public static IEditorTileModifierType SelectedTileModifier;

    public static int SelectedTileMainModifierCategoryIndex;
    public static int SelectedTileAttributeModifierIndex;
    public static int SelectedTileBackgroundModifierIndex;
    public static int SelectedTileTransformationTriggererIndex;
    public static int SelectedTileAreaModifierIndex;

    public static void ToggleEditorMode()
    {
        if (_inEditor)
        {
            CloseEditor();
            return;
        }
        OpenEditor();
    }

    public static void OpenEditor()
    {
        _inEditor = true;
        EditorCanvasUI.Instance.InitialiseEditor();
        EditorWorldContainer.Instance.InitialiseEditor();
        Logger.Log($"Our current scene type is {PersistentGameManager.CurrentSceneType}. We are opening the editor");
        switch (PersistentGameManager.CurrentSceneType)
        {
            case SceneType.Overworld:
                EditorCanvasUI.Instance.OverworldModificationPanel.GenerateTiles();
                break;
            case SceneType.Maze:
                EditorCanvasUI.Instance.MazeModificationPanel.GenerateTiles();
                break;
            default:
                Logger.Error($"Unknown scene type {PersistentGameManager.CurrentSceneType} is not implemented");
                break;
        }

        if (GameRules.GamePlayerType == GamePlayerType.SplitScreenMultiplayer)
        {
            CameraManager.Instance.CreateOneCamera();
        }
    }

    public static void CloseEditor()
    {
        _inEditor = false;
        EditorCanvasUI.Instance.CloseEditor();
        EditorWorldContainer.Instance.CloseEditor();

        if (GameRules.GamePlayerType == GamePlayerType.SplitScreenMultiplayer)
        {
            CameraManager.Instance.CreateTwoCameras();
        }
    }
}
