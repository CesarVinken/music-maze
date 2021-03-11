public abstract class TileAttributePlacer<T> where T : Tile
{
    public abstract T Tile { get; set; }

    public abstract ITileAttribute InstantiateTileAttributeGO<U>() where U : ITileAttribute;
}