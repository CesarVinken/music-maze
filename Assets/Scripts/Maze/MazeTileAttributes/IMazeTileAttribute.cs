public interface IMazeTileAttribute
{
    int SortingOrderBase { get; set; }

    void SetTile(Tile tile);
    void Remove();
}
