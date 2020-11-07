using System;

[Serializable]
public class LevelNameData
{
    public string LevelName = "";
    public bool IsPlayable;

    public LevelNameData()
    {

    }

    public LevelNameData(string levelName)
    {
        LevelName = levelName;
        IsPlayable = true;
    }

    public LevelNameData WithName(string levelName)
    {
        LevelName = levelName;
        return this;
    }

    public LevelNameData WithPlayability(bool isPlayable)
    {
        IsPlayable = isPlayable;
        return this;
    }

    public void ToggleSelection(bool isPlayable)
    {
        IsPlayable = isPlayable;
    }
}