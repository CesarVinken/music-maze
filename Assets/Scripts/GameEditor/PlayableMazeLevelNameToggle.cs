﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayableMazeLevelNameToggle : MonoBehaviour
{
    private LevelNameData _levelNameData;
    public Toggle Toggle;
    [SerializeField] private Text _levelNameToggleLabel;

    public void Awake()
    {
        Guard.CheckIsNull(_levelNameToggleLabel, "LevelNameToggleLabel", gameObject);    
    }

    public void Initialise(LevelNameData levelNameData)
    {
        _levelNameData = levelNameData;

        gameObject.name = $"{_levelNameData.LevelName}Toggle";
        _levelNameToggleLabel.text = _levelNameData.LevelName;
        Toggle.isOn = levelNameData.IsPlayable;
    }

    public void ToggleLevelPlayability(bool isPlayable)
    {
        _levelNameData.IsPlayable = isPlayable;
        Logger.Log($"Toggled playability of the maze level {_levelNameData.LevelName} to {isPlayable}");
    }
}