﻿using DataSerialisation;
using System.Collections.Generic;
using UnityEngine;

public class PlayableLevelsPanel : MonoBehaviour
{
    public static PlayableLevelsPanel Instance;

    public GameObject PlayableLevelNameTogglePrefab;
    public Transform PlayableLevelListContainer;

    public Dictionary<PlayableMazeLevelNameToggle, MazeLevelNameData> LevelNameToggleData = new Dictionary<PlayableMazeLevelNameToggle, MazeLevelNameData>();

    public static bool IsOpen = false;

    public void Awake()
    {
        Guard.CheckIsNull(PlayableLevelNameTogglePrefab, "PlayableLevelNameTogglePrefab", gameObject);
        Guard.CheckIsNull(PlayableLevelListContainer, "PlayableLevelListContainer", gameObject);

        Instance = this;
    }

    public void Update()
    {
        if (IsOpen)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ClosePanel();
            }
        }
    }

    public void OnEnable()
    {
        MazeLevelNamesData levelNamesData = new JsonMazeLevelListFileReader().ReadData<MazeLevelNamesData>();
        
        LevelNameToggleData.Clear();

        for (int i = 0; i < levelNamesData.LevelNames.Count; i++)
        {
            MazeLevelNameData levelNameData = levelNamesData.LevelNames[i];

            GameObject levelNameToggleGO = Instantiate(PlayableLevelNameTogglePrefab, PlayableLevelListContainer);

            PlayableMazeLevelNameToggle playableLevelNameToggle = levelNameToggleGO.GetComponent<PlayableMazeLevelNameToggle>();
            playableLevelNameToggle.Initialise(levelNameData);

            LevelNameToggleData.Add(playableLevelNameToggle, levelNameData);
        }

        IsOpen = true;
    }

    public void OnDisable()
    {
        // Destroy all toggles
        foreach (Transform child in PlayableLevelListContainer.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        IsOpen = false;
    }

    //Save changes to which levels are playable to the levels.json file
    public void SaveChanges()
    {
        JsonMazeLevelListFileWriter jsonMazeLevelListFileWriter = new JsonMazeLevelListFileWriter();
        MazeLevelNamesData levelNamesData = new MazeLevelNamesData();

        foreach (KeyValuePair<PlayableMazeLevelNameToggle, MazeLevelNameData> mazeLevelNameToggleData in LevelNameToggleData)
        {
            MazeLevelNameData levelNameData = new MazeLevelNameData()
                .WithName(mazeLevelNameToggleData.Value.LevelName)
                .WithPlayability(mazeLevelNameToggleData.Key.Toggle.isOn);

            levelNamesData.LevelNames.Add(levelNameData);
        }

        jsonMazeLevelListFileWriter.SerialiseData(levelNamesData);

        Logger.Log("Playable level selection changes were saved.");

        ClosePanel();
    }

    public void ClosePanel()
    {
        gameObject.SetActive(false);
    }
}
