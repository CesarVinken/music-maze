using System.Collections.Generic;

public abstract class TileBackgroundPlacer<T> where T : Tile
{
    public abstract T Tile { get; set; }

    public abstract void PlacePath(IPathType tilePathType, TileConnectionScoreInfo pathConnectionScoreInfo);

    public void PlacePathVariation(TilePath mazeTilePath)
    {
        //return only connections that were updated
        List<TilePath> updatedPathConnections = NeighbourTileCalculator.GetUpdatedTileModifiersForVariation(Tile, mazeTilePath, mazeTilePath.TilePathType.ToString());

        //update the sprites with the new variations
        for (int i = 0; i < updatedPathConnections.Count; i++)
        {
            updatedPathConnections[i].WithConnectionScoreInfo(new TileConnectionScoreInfo(updatedPathConnections[i].ConnectionScore, updatedPathConnections[i].SpriteNumber));
        }
    }

    public abstract void PlaceBaseBackground(IBaseBackgroundType baseBackgroundType);
}
