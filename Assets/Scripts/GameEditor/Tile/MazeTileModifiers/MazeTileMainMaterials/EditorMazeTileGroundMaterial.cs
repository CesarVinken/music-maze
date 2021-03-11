public class EditorMazeTileGroundMaterial : EditorMazeTileMainMaterialModifier
{
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
