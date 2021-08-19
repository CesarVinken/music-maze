using System.Collections.Generic;

public class MazeTileFullTransformationMapper
{
    private static List<EditorMazeTile> _checkedNeighbours = new List<EditorMazeTile>();

    public EditorMazeTile SelectedTile;

    public static void GenerateFullTileTransformationMap()
    {
        Logger.Log("generate tile full transformation map");

        // remove existing tile overlays
        RemoveCurrentTileOverlay();

        //go over all tiles and if tile is non-markable (or bridge or spawnpoint), empty transformation trigger list and assign transformation triggers based on adjacent tiles
        for (int i = 0; i < MazeLevelGameplayManager.Instance.EditorLevel.Tiles.Count; i++)
        {
            EditorMazeTile tile = MazeLevelGameplayManager.Instance.EditorLevel.Tiles[i] as EditorMazeTile;
            if (tile.Markable || tile.TryGetAttribute<PlayerSpawnpoint>()) continue;

            tile.BeautificationTriggerers.Clear();
            _checkedNeighbours.Clear();
            tile.BeautificationTriggerers = FindAllMarkableNeighbours(tile, 0);
        }
    }

    private static List<EditorMazeTile> FindAllMarkableNeighbours(EditorMazeTile tile, int iteration, List<EditorMazeTile> foundSoFar = null)
    {
        if (foundSoFar == null)
        {
            foundSoFar = new List<EditorMazeTile>();
        }

        if (iteration >= 4)
        {
            return foundSoFar;
        }

        List<EditorMazeTile> newNeighbouringTiles = new List<EditorMazeTile>();

        foreach (KeyValuePair<Direction, Tile> neighbour in tile.Neighbours)
        {
            if (!neighbour.Value) continue;

            EditorMazeTile neighbourTile = neighbour.Value as EditorMazeTile;
            if (!foundSoFar.Contains(neighbourTile as EditorMazeTile))
            {
                if (neighbourTile.Markable || neighbourTile.TryGetAttribute<BridgePiece>() || neighbourTile.TryGetAttribute<PlayerSpawnpoint>())
                {
                    foundSoFar.Add(neighbourTile);
                }
                if (!_checkedNeighbours.Contains(neighbourTile))
                {
                    newNeighbouringTiles.Add(neighbourTile);
                }
            }
        }

        if (foundSoFar.Count > 0)
            return foundSoFar;

        // Recursion starts here.
        foreach (EditorMazeTile editorTile in newNeighbouringTiles)
        {
            List<EditorMazeTile> markableNeighbours = FindAllMarkableNeighbours(editorTile, iteration + 1, foundSoFar);

            for (int k = 0; k < markableNeighbours.Count; k++)
            {
                if (!foundSoFar.Contains(markableNeighbours[k] as EditorMazeTile))
                {
                    foundSoFar.Add(markableNeighbours[k]);
                }
            }
        }

        return foundSoFar;
    }

    private static void RemoveCurrentTileOverlay()
    {
        EditorMazeTile overlayTile = EditorTileSelector.Instance.OverlayImageTile as EditorMazeTile;
        
        if (overlayTile != null)
        {
            overlayTile.SetTileOverlayImage(TileOverlayMode.Empty);
            for (int i = 0; i < overlayTile.BeautificationTriggerers.Count; i++)
            {
                overlayTile.BeautificationTriggerers[i].SetTileOverlayImage(TileOverlayMode.Empty);
            }
        }
    }
}
