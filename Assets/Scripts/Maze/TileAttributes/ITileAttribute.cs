public interface ITileAttribute
{
    int SortingOrderBase { get; set; }

    void SetTile(Tile tile);
    void Remove();
}
