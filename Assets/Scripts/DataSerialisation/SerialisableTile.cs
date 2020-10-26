using System;

[Serializable]
public class SerialisableTile
{
    public string Id;
    public bool Walkable;   // TODO: turn into enum/int value with different possible wallpieces. Eg. 0 = Walkable, 1 = CornerLeft, 2 = CorderRight etc.
    public int GridLocationX;
    public int GridLocationY;

    public SerialisableTile(Tile tile)
    {
        Id = tile.TileId;
        Walkable = tile.Walkable;
        GridLocationX = tile.GridLocation.X;
        GridLocationY = tile.GridLocation.Y;
    }
}