using System.Collections.Generic;

public abstract class TileBackgroundPlacer<T> where T : Tile
{
    public abstract T Tile { get; set; }

    public abstract void PlacePath(IPathType tilePathType, TileConnectionScoreInfo pathConnectionScoreInfo);
    public abstract void PlaceWater(IBaseBackgroundType waterType, TileConnectionScoreInfo pathConnectionScoreInfo);
    public abstract void PlaceGround(IBaseBackgroundType groundType, TileConnectionScoreInfo pathConnectionScoreInfo);

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

    public void PlaceGroundVariation(TileBaseGround tileGround)
    {
        Logger.Log(tileGround.TileGroundType);
        //return only connections that were updated
        List<TileBaseGround> updatedGroundConnections = NeighbourTileCalculator.GetUpdatedTileModifiersForVariation(Tile, tileGround, tileGround.TileGroundType.ToString());
        Logger.Warning($"Number of path connections to update with variations: {updatedGroundConnections.Count}");

        //update the sprites with the new variations
        for (int i = 0; i < updatedGroundConnections.Count; i++)
        {
            updatedGroundConnections[i].WithConnectionScoreInfo(new TileConnectionScoreInfo(updatedGroundConnections[i].ConnectionScore, updatedGroundConnections[i].SpriteNumber));
        }
    }

    public abstract U PlaceBackground<U>() where U : ITileBackground;
    public abstract void PlaceCornerFiler(TileCorner tileCorner);
}
