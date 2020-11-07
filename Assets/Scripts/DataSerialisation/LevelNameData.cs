using System;

[Serializable]
public class LevelNameData
{
    public string LevelName;
    public bool IsSelected;

    public LevelNameData(string levelName)
    {
        LevelName = levelName;
        IsSelected = true;
    }

    public void ToggleSelection(bool isSelected)
    {
        IsSelected = isSelected;
    }
}