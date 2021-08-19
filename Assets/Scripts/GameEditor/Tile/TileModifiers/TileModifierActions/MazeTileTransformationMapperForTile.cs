using System.Collections.Generic;
using UnityEngine;

public class MazeTileTransformationMapperForTile : MonoBehaviour
{
    public static void GenerateTileTransformationMapForTile()
    {
        Logger.Log("GenerateTileTransformationMapForTile");
        EditorMazeTile selectedTile = EditorTileSelector.Instance.CurrentlySelectedTile as EditorMazeTile; // Currently only works on maze levels, not on overworld

        if (selectedTile == null) return;

        // remove drawn overlay for old triggerers
        RemoveCurrentTileOverlay(EditorTileSelector.Instance.OverlayImageTile);

        //selectedTile.BeautificationTriggerers.Clear();

        selectedTile.BeautificationTriggerers = GetTransformationTriggerers(selectedTile);

        // redraw tiles' transformation status
        RedrawTileOverlay(selectedTile);
    }

    private static List<EditorMazeTile> GetTransformationTriggerers(EditorMazeTile selectedTile)
    {
        List<EditorMazeTile> foundSoFar = new List<EditorMazeTile>();

        foreach (KeyValuePair<Direction, Tile> neighbour in selectedTile.Neighbours)
        {
            if (!neighbour.Value) continue; // if there is no neighbour

            EditorMazeTile neighbourTile = neighbour.Value as EditorMazeTile;
            if (!foundSoFar.Contains(neighbourTile as EditorMazeTile))
            {
                if (neighbourTile.Markable || neighbourTile.TryGetAttribute<BridgePiece>() || neighbourTile.TryGetAttribute<PlayerSpawnpoint>())
                {
                    foundSoFar.Add(neighbourTile);
                }
            }
        }

        return foundSoFar;
    }

    private static void RemoveCurrentTileOverlay(EditorMazeTile currentOverlayTile)
    {
        if (currentOverlayTile != null)
        {
            currentOverlayTile.SetTileOverlayImage(TileOverlayMode.Empty);
            for (int i = currentOverlayTile.BeautificationTriggerers.Count - 1; i >= 0; i--)
            {
                currentOverlayTile.BeautificationTriggerers[i].SetTileOverlayImage(TileOverlayMode.Empty);
                currentOverlayTile.BeautificationTriggerers.Remove(currentOverlayTile.BeautificationTriggerers[i]);
            }
        }
    }

    private static void RedrawTileOverlay(EditorMazeTile selectedTile)
    {
        selectedTile.SetTileOverlayImage(TileOverlayMode.Green);
        EditorTileSelector.Instance.OverlayImageTile = selectedTile;

        for (int i = 0; i < selectedTile.BeautificationTriggerers.Count; i++)
        {
            selectedTile.BeautificationTriggerers[i].SetTileOverlayImage(TileOverlayMode.Blue); // blue = triggerer
        }
    }
}
