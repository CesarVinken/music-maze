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
        LevelName = levelName.ToLower().Replace(" ", "-");
        IsPlayable = true;
    }

    public LevelNameData WithName(string levelName)
    {
        LevelName = levelName.ToLower().Replace(" ", "-");
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