using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EditorFerryRouteTileAttribute : EditorMazeTileAttributeModifier, IWaterMaterialModifier
{
    public override string Name { get => "Ferry Route"; }

    public override void PlaceAttribute(EditorMazeTile tile)
    {
        Logger.Log("Try place Ferry Route");
        if (tile.TileMainMaterial.GetType() != typeof(WaterMainMaterial))
        {
            return;
        }

        if (FerryRouteDrawingModeAccessor.InDrawingMode)
        {
            return;
        }

        EditorMazeTileAttributePlacer tileAttributePlacer = new EditorMazeTileAttributePlacer(tile);
        MazeTileAttributeRemover tileAttributeRemover = new MazeTileAttributeRemover(tile);

        ITileAttribute ferryRouteOnTile = (FerryRoute)tile.GetAttributes().FirstOrDefault(attribute => attribute is FerryRoute);
        if (ferryRouteOnTile == null)
        {
            // make sure this tile is not already part of another FerryRoute's route
            EditorMazeLevel editorMazeLevel = GameManager.Instance.CurrentEditorLevel as EditorMazeLevel;
            if (editorMazeLevel == null)
            {
                Logger.Error("Could not find an instance of the editor maze level");
            }

            for (int i = 0; i < editorMazeLevel.FerryRoutes.Count; i++)
            {
                FerryRoute existingFerryRoute = editorMazeLevel.FerryRoutes[i];
                List<FerryRoutePoint> ferryRoutePoints = existingFerryRoute.GetFerryRoutePoints();
                for (int j = 0; j < ferryRoutePoints.Count; j++)
                {
                    if (ferryRoutePoints[j].Tile.TileId.Equals(tile.TileId))
                    {
                        return; // return because we found that this tile is already a point on a ferry route
                    }
                }
            }

            tileAttributeRemover.Remove<BridgePiece>();

            tileAttributePlacer.PlaceFerryRoute();
            return;
        }

        tileAttributeRemover.Remove<FerryRoute>();
    }

    public override Sprite GetSprite()
    {
        return EditorCanvasUI.Instance.TileAttributeIcons[9];
    }

    public override void InstantiateModifierActions()
    {
        Logger.Log("Load actions for ferry route");
        GameObject.Instantiate(EditorCanvasUI.Instance.ToggleFerryRouteDrawingModePrefab, EditorMazeTileModificationPanel.Instance.TileModifierActionsContainer);
    }
}
