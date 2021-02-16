using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MazeTileTransformationMapper : MonoBehaviour
{
    private static List<EditorMazeTile> _checkedNeighbours = new List<EditorMazeTile>();

    public static void GenerateTileTransformationMap()
    {
        Logger.Log("generate tile transformation map");
        //go over all tiles and if tile is non-markable, empty transformation trigger list and assign transformation triggers based on adjacent tiles

        for (int i = 0; i < MazeLevelManager.Instance.EditorLevel.Tiles.Count; i++)
        {
            EditorMazeTile tile = MazeLevelManager.Instance.EditorLevel.Tiles[i];
            if (tile.Markable || tile.TileAttributes.OfType<PlayerSpawnpoint>().Any()) continue;

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

        foreach (KeyValuePair<ObjectDirection, Tile> item in tile.Neighbours)
        {
            EditorMazeTile neighbourTile = item.Value as EditorMazeTile;
            if (!foundSoFar.Contains(neighbourTile as EditorMazeTile))
            {
                if (neighbourTile.Markable)
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
}
