using System.Collections.Generic;

public abstract class TileBackgroundPlacer<T> where T : Tile
{
    public abstract T Tile { get; set; }

    public abstract void PlacePath(IPathType tilePathType, TileConnectionScoreInfo pathConnectionScoreInfo);
    public abstract void PlaceWater(IBaseBackgroundType waterType, TileConnectionScoreInfo pathConnectionScoreInfo);

    public void PlacePathVariation(TilePath tilePath)
    {
        //return only connections that were updated
        List<TilePath> updatedPathConnections = NeighbourTileCalculator.GetUpdatedTileModifiersForVariation(Tile, tilePath, tilePath.TilePathType.ToString());

        //update the sprites with the new variations
        for (int i = 0; i < updatedPathConnections.Count; i++)
        {
            updatedPathConnections[i].WithConnectionScoreInfo(new TileConnectionScoreInfo(updatedPathConnections[i].ConnectionScore, updatedPathConnections[i].SpriteNumber));
        }
    }

    public void PlaceWaterVariation(TileWater tileWater)
    {
        //return only connections that were updated
        List<TileWater> updatedWaterConnections = NeighbourTileCalculator.GetUpdatedTileModifiersForVariation(Tile, tileWater, tileWater.TileWaterType.ToString());

        //update the sprites with the new variations
        for (int i = 0; i < updatedWaterConnections.Count; i++)
        {
            updatedWaterConnections[i].WithConnectionScoreInfo(new TileConnectionScoreInfo(updatedWaterConnections[i].ConnectionScore, updatedWaterConnections[i].SpriteNumber));
        }
    }

    public abstract U PlaceBackground<U>() where U : ITileBackground;
}
