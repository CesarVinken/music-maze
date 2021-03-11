
public class MazeTileMainMaterialPlacer<T> : TileMainMaterialPlacer<T> where T : MazeTile
{
    public override T Tile { get; set; }

    public void SetMainMaterialForTile(ITileMainMaterial tileMainMaterial)
    {
        Tile.TileMainMaterial = tileMainMaterial;
    }
}
