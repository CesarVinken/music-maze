using UnityEngine;

public class OverworldNameData
{
    public string OverworldName = "";
    public bool IsPlayable;

    public OverworldNameData()
    {

    }

    public OverworldNameData(string overworldName)
    {
        OverworldName = overworldName.ToLower().Replace(" ", "-");
        IsPlayable = true;
    }

    public OverworldNameData WithName(string overworldName)
    {
        OverworldName = overworldName.ToLower().Replace(" ", "-");
        return this;
    }

    public OverworldNameData WithPlayability(bool isPlayable)
    {
        IsPlayable = isPlayable;
        return this;
    }

    public void ToggleSelection(bool isPlayable)
    {
        IsPlayable = isPlayable;
    }
}
