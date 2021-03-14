using UnityEngine;

public class EditorOverworldWaterMaterial : EditorOverworldTileMainMaterialModifier
{
    public override string Name { get => "Water"; }
    private Sprite _sprite = EditorCanvasUI.Instance.TileMainMaterialIcons[1];
    public override Sprite Sprite { get => _sprite; set => _sprite = value; }

    public override Sprite GetSprite()
    {
        return Sprite;
    }

    public override void PlaceMainMaterial(EditorOverworldTile tile)
    {
        EditorOverworldTileMainMaterialPlacer tileMainMaterialPlacer = new EditorOverworldTileMainMaterialPlacer(tile);
        //MazeTileMainMaterialRemover tileMainMaterialRemover = new MazeTileMainMaterialRemover(tile);

        ITileMainMaterial waterMaterial = (WaterMainMaterial)tile.TileMainMaterial;
        if (waterMaterial == null)
        {
            Logger.Log("Place Water material");
            //MazeTileAttributeRemover tileAttributeRemover = new MazeTileAttributeRemover(tile);
            //tileAttributeRemover.RemoveTileObstacle();

            //tileBackgroundPlacer.PlacePath(new MazeLevelDefaultPathType());
            return;
        }

        Logger.Log("Remove water material or do nothing?");

        //tileBackgroundRemover.RemovePath();
    }
}
