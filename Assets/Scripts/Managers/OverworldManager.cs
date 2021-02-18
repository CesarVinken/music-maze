﻿using System;
using UnityEngine;

public class OverworldManager : MonoBehaviour
{
    public static OverworldManager Instance;

    public InGameOverworld Overworld;
    public EditorOverworld EditorOverworld;

    public GameObject EditorTilePrefab;
    public GameObject InGameTilePrefab;
    public GameObject TileBaseBackgroundPrefab;
    public GameObject TilePathPrefab;
    public GameObject PlayerSpawnpointPrefab;

    public void Awake()
    {
        Instance = this;

        Guard.CheckIsNull(EditorTilePrefab, "EditorTilePrefab", gameObject);
        Guard.CheckIsNull(InGameTilePrefab, "InGameTilePrefab", gameObject);
        Guard.CheckIsNull(TileBaseBackgroundPrefab, "TileBaseBackgroundPrefab", gameObject);
        Guard.CheckIsNull(TilePathPrefab, "TilePathPrefab", gameObject);
        Guard.CheckIsNull(PlayerSpawnpointPrefab, "PlayerSpawnpointPrefab", gameObject);
    }

    public void UnloadOverworld()
    {
        if (Overworld == null) return;

        if (TilesContainer.Instance)
        {
            Destroy(TilesContainer.Instance.gameObject);
        }

        TilesContainer.Instance = null;

        CharacterManager.Instance.UnloadCharacters();
        SceneObjectManager.Instance.UnloadSceneObjects();

        Logger.Log(Logger.Initialisation, "Unload Overworld {0}", Overworld);
        Overworld.Tiles.Clear();
        Overworld.TilesByLocation.Clear();
        CameraController.Instance.ResetCamera();
        Overworld = null;
    }

    public void SetupOverworld(OverworldData overworldData)
    {
        Overworld = InGameOverworld.Create(overworldData);

        InitialiseTileAttributes();

        //Logger.Log("Start scan...");
        //IEnumerator coroutine = ScanCoroutine();

        //StartCoroutine(coroutine);
    }

    public void SetupOverworldForEditor(OverworldData overworldData)
    {
        EditorOverworld = EditorOverworld.Create(overworldData);

        InitialiseEditorTileBackgrounds();
        InitialiseEditorTileAttributes();

        MainCanvas.Instance.BlackOutSquare.ResetToDefault();
        CameraController.Instance.ResetCamera();
        CameraController.Instance.SetPanLimits(EditorOverworld.LevelBounds);
    }

    private void InitialiseTileAttributes()
    {
        for (int i = 0; i < Overworld.Tiles.Count; i++)
        {
            InGameOverworldTile tile = Overworld.Tiles[i];
            tile.InitialiseTileAttributes();
        }
    }

    private void InitialiseEditorTileAttributes()
    {
        for (int i = 0; i < EditorOverworld.Tiles.Count; i++)
        {
            EditorOverworldTile tile = EditorOverworld.Tiles[i];
            tile.InitialiseTileAttributes();
        }
    }

    private void InitialiseEditorTileBackgrounds()
    {
        for (int i = 0; i < EditorOverworld.Tiles.Count; i++)
        {
            EditorOverworldTile tile = EditorOverworld.Tiles[i];
            tile.InitialiseTileBackgrounds();
        }
    }

    public GameObject GetTileAttributePrefab<T>() where T : ITileAttribute
    {
        switch (typeof(T))
        {
            //case Type tileObstacle when tileObstacle == typeof(TileObstacle):
            //    return TileObstaclePrefab;
            case Type playerSpawnpoint when playerSpawnpoint == typeof(PlayerSpawnpoint):
                return PlayerSpawnpointPrefab;
            default:
                Logger.Error($"Could not find a prefab for the tile attribute type of {typeof(T)}");
                return null;
        }
    }

}
