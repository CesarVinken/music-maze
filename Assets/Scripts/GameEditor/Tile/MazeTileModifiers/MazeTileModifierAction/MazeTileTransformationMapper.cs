using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MazeTileTransformationMapper : MonoBehaviour
{
    private static List<EditorTile> _checkedNeighbours = new List<EditorTile>();

    public static void GenerateTileTransformationMap()
    {
        Logger.Log("generate tile transformation map");
        //go over all tiles and if tile is non-markable, empty transformation trigger list and assign transformation triggers based on adjacent tiles

        for (int i = 0; i < MazeLevelManager.Instance.EditorLevel.Tiles.Count; i++)
        {
            EditorTile tile = MazeLevelManager.Instance.EditorLevel.Tiles[i];
            if (tile.Markable || tile.MazeTileAttributes.OfType<PlayerSpawnpoint>().Any()) continue;

            tile.TransformationTriggerers.Clear();
            _checkedNeighbours.Clear();

            tile.TransformationTriggerers = FindAllMarkableNeighbours(tile, 0);
        }
    }

    private static List<EditorTile> FindAllMarkableNeighbours(EditorTile tile, int iteration, List<EditorTile> foundSoFar = null)
    {
        if (foundSoFar == null)
        {
            foundSoFar = new List<EditorTile>();
        }

        if (iteration >= 4)
        {
            return foundSoFar;
        }

        List<EditorTile> newNeighbouringTiles = new List<EditorTile>();

        foreach (KeyValuePair<ObjectDirection, Tile> item in tile.Neighbours)
        {
            EditorTile neighbourTile = item.Value as EditorTile;
            if (!foundSoFar.Contains(neighbourTile as EditorTile))
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
        foreach (EditorTile editorTile in newNeighbouringTiles)
        {
            List<EditorTile> markableNeighbours = FindAllMarkableNeighbours(editorTile, iteration + 1, foundSoFar);

            for (int k = 0; k < markableNeighbours.Count; k++)
            {
                if (!foundSoFar.Contains(markableNeighbours[k] as EditorTile))
                {
                    foundSoFar.Add(markableNeighbours[k]);
                }

            }
        }

        return foundSoFar;
    }
}
