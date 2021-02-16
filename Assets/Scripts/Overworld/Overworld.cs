using UnityEngine;

public class Overworld
{
    public string OverworldName;

    protected GridLocation _levelBounds = new GridLocation(0, 0);
    public GridLocation LevelBounds { get => _levelBounds; set => _levelBounds = value; }

    protected GameObject _overworldContainer;

}
