using UnityEngine;

public class EditorMazeTileGroundMaterial : EditorMazeTileMainMaterialModifier
{
    public override string Name { get => "Ground"; }
    private Sprite _sprite = EditorCanvasUI.Instance.TileMainMaterialIcons[0];
    public override Sprite Sprite { get => _sprite; set => _sprite = value; }

    public override Sprite GetSprite()
    {
        return Sprite;
    }

    public override void PlaceMainMaterial(EditorMazeTile tile)
    {
        EditorMazeTileMainMaterialPlacer tileMainMaterialPlacer = new EditorMazeTileMainMaterialPlacer(tile);
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
