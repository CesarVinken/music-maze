﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OverworldTileBackgroundRemover : TileBackgroundRemover
{
    private EditorOverworldTile _tile;

    public OverworldTileBackgroundRemover(EditorOverworldTile tile)
    {
        _tile = tile;
    }

    public override void RemovePath()
    {
        OverworldTilePath overworldTilePath = (OverworldTilePath)_tile.TileBackgrounds.FirstOrDefault(background => background is OverworldTilePath);
        if (overworldTilePath == null) return;

        Logger.Log(overworldTilePath.TilePathType);
        IPathType overworldTilePathType = overworldTilePath.TilePathType;
        int oldConnectionScore = overworldTilePath.ConnectionScore;

        // If needed, place a background in the gap that the removed path left
        if (oldConnectionScore == NeighbourTileCalculator.ConnectionOnAllSidesScore)
        {
            EditorOverworldTileBackgroundPlacer tileBackgroundPlacer = new EditorOverworldTileBackgroundPlacer(_tile);
            tileBackgroundPlacer.PlaceBaseBackground(new OverworldDefaultBaseBackgroundType());
        }

        _tile.TileBackgrounds.Remove(overworldTilePath);
        overworldTilePath.Remove();

        //After removing tile, check with neighbour tiles if wall connections should be updated
        foreach (KeyValuePair<ObjectDirection, Tile> neighbour in _tile.Neighbours)
        {
            TilePath overworldTilePathOnNeighbour = neighbour.Value.TryGetTilePath();

            if (overworldTilePathOnNeighbour == null) continue;

            int oldConnectionScoreOnNeighbour = overworldTilePathOnNeighbour.ConnectionScore;

            Logger.Warning($"We will now look for connections for neighbour {neighbour.Value.GridLocation.X},{neighbour.Value.GridLocation.Y}, which is {neighbour.Key} of {_tile.GridLocation.X},{_tile.GridLocation.Y}");
            TileConnectionScoreInfo overworldTilePathConnectionScoreOnNeighbourInfo = NeighbourTileCalculator.MapNeighbourPathsOfTile(neighbour.Value, overworldTilePathType);
            Logger.Log($"We calculated an path connection type score of {overworldTilePathConnectionScoreOnNeighbourInfo.RawConnectionScore} for location {neighbour.Value.GridLocation.X}, {neighbour.Value.GridLocation.Y}");

            //update connection score on neighbour
            overworldTilePathOnNeighbour.WithConnectionScoreInfo(overworldTilePathConnectionScoreOnNeighbourInfo);

            //Add background where needed
            if (oldConnectionScoreOnNeighbour == NeighbourTileCalculator.ConnectionOnAllSidesScore && overworldTilePathConnectionScoreOnNeighbourInfo.RawConnectionScore != NeighbourTileCalculator.ConnectionOnAllSidesScore)
            {
                EditorOverworldTileBackgroundPlacer tileBackgroundPlacer = new EditorOverworldTileBackgroundPlacer(neighbour.Value as EditorOverworldTile);
                tileBackgroundPlacer.PlaceBaseBackground(new OverworldDefaultBaseBackgroundType());
            }
        }
    }

    public override void RemoveBaseBackground(IBaseBackgroundType overworldTileBaseBackgroundType)
    {
        OverworldTileBaseBackground overworldTileBaseBackground = (OverworldTileBaseBackground)_tile.TileBackgrounds.FirstOrDefault(background => background is OverworldTileBaseBackground);

        if (overworldTileBaseBackground == null) return;

        _tile.TileBackgrounds.Remove(overworldTileBaseBackground);
        overworldTileBaseBackground.Remove();
    }
}
