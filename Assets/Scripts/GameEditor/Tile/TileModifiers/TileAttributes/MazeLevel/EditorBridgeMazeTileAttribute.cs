using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EditorBridgeMazeTileAttribute : EditorMazeTileAttributeModifier, IWaterMaterialModifier
{
    public override string Name { get => "Bridge"; }

    public override void PlaceAttribute(EditorMazeTile tile)
    {
        if (tile.TileMainMaterial.GetType() != typeof(WaterMainMaterial))
        {
            Logger.Log("TODO: Check if this works. Bridge can also be placed on any tile with a coastline");
            return;
        }

        EditorMazeTileAttributePlacer tileAttributePlacer = new EditorMazeTileAttributePlacer(tile);
        MazeTileAttributeRemover tileAttributeRemover = new MazeTileAttributeRemover(tile);

        BridgePiece bridgePiece = (BridgePiece)tile.GetAttributes().FirstOrDefault(attribute => (attribute is BridgePiece));
        if (bridgePiece == null)
        {
            tileAttributePlacer.CreateBridgePiece(BridgePieceDirection.Horizontal);
            return;
        }
        else if(bridgePiece.BridgePieceDirection == BridgePieceDirection.Horizontal)
        {
            tileAttributeRemover.RemoveBridgePiece();
            tileAttributePlacer.CreateBridgePiece(BridgePieceDirection.Vertical);
        }
        else
        {
            tileAttributeRemover.RemoveBridgePiece();
        }
    }

    public override void PlaceAttributeVariation(EditorMazeTile tile)
    {
        Logger.Log("Place bridge variation. Todo.");
        //ITileAttribute tileObstacle = (TileObstacle)tile.GetAttributes().FirstOrDefault(attribute => attribute is TileObstacle);

        //if (tileObstacle == null) return; // only place variation if there is already an obstacle

        //EditorMazeTileAttributePlacer tileAttributePlacer = new EditorMazeTileAttributePlacer(tile);
        //tileAttributePlacer.PlaceTileObstacleVariation((TileObstacle)tileObstacle);
    }

    public override Sprite GetSprite()
    {
        return EditorCanvasUI.Instance.TileAttributeIcons[6];
    }
}
