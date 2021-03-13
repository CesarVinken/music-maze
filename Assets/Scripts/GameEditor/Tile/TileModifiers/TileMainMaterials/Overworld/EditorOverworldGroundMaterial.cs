

using UnityEngine;

public class EditorOverworldGroundMaterial : EditorOverworldTileMainMaterialModifier
{
    public override string Name => "Ground";

    private Sprite _sprite = EditorCanvasUI.Instance.TileMainMaterialIcons[0];
    public override Sprite Sprite { get => _sprite; set => _sprite = value; }

    public override Sprite GetSprite()
    {
        return Sprite;
    }

    public override void PlaceMainMaterial(EditorOverworldTile tile)
    {
        EditorOverworldTileMainMaterialPlacer tileMainMaterialPlacer = new EditorOverworldTileMainMaterialPlacer(tile);
        //MazeTileMainMaterialRemover tileMainMaterialRemover = new MazeTileMainMaterialRemover(tile);

        ITileMainMaterial groundMaterial = (GroundMainMaterial)tile.TileMainMaterial;
        if (groundMaterial == null)
        {
            Logger.Log("Place Ground material");
            //MazeTileAttributeRemover tileAttributeRemover = new MazeTileAttributeRemover(tile);
            //tileAttributeRemover.RemoveTileObstacle();

            //tileBackgroundPlacer.PlacePath(new MazeLevelDefaultPathType());
            return;
        }

        Logger.Log("Remove ground material or do nothing?");

        //tileBackgroundRemover.RemovePath();
    }
}
