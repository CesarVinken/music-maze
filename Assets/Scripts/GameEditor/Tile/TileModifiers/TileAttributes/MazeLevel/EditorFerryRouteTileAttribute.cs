
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

        EditorMazeTileAttributePlacer tileAttributePlacer = new EditorMazeTileAttributePlacer(tile);
        MazeTileAttributeRemover tileAttributeRemover = new MazeTileAttributeRemover(tile);

        ITileAttribute ferryRoute = (FerryRoute)tile.GetAttributes().FirstOrDefault(attribute => attribute is FerryRoute);
        if (ferryRoute == null)
        {
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
