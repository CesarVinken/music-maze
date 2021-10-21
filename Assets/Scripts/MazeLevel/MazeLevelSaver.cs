using DataSerialisation;

public class MazeLevelSaver
{
    private string _mazeName;

    public void Save(string mazeName)
    {
        _mazeName = mazeName;

        CheckForTilesWithoutTransformationTriggerers();

        SaveMazeLevelData();
        AddMazeToMazeList();

        GameManager.Instance.CurrentEditorLevel.UnsavedChanges = false;

        Logger.Log(Logger.Datawriting, "Level {0} Saved.", _mazeName);
    }

    private void CheckForTilesWithoutTransformationTriggerers()
    {
        for (int i = 0; i < MazeLevelGameplayManager.Instance.EditorLevel.Tiles.Count; i++)
        {
            EditorMazeTile tile = MazeLevelGameplayManager.Instance.EditorLevel.Tiles[i] as EditorMazeTile;

            if (!tile.Markable && tile.BeautificationTriggerers.Count == 0)
            {
                tile.SetTileOverlayImage(TileOverlayMode.Yellow);
                Logger.Warning($"No transformation triggerer was set up for the tile at {tile.GridLocation.X},{tile.GridLocation.Y}");
            }
        }
    }

    private void SaveMazeLevelData()
    {
        MazeLevelData mazeLevelData = new MazeLevelData(MazeLevelGameplayManager.Instance.EditorLevel).WithName(_mazeName);
        JsonMazeLevelFileWriter fileWriter = new JsonMazeLevelFileWriter();
        fileWriter.SerialiseData(mazeLevelData);
    }


    private void AddMazeToMazeList()
    {
        MazeLevelNamesData levelNamesData = new MazeLevelNamesData(_mazeName).AddLevelName(_mazeName);

        JsonMazeLevelListFileWriter fileWriter = new JsonMazeLevelListFileWriter();
        fileWriter.SerialiseData(levelNamesData);
    }

}
