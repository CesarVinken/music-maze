using DataSerialisation;

public class OverworldSaver
{
    private string _overworldName;

    public void Save(string overworldName)
    {
        _overworldName = overworldName;

        SaveOverworldData();
        AddOverworldToOverworldList();

        GameManager.Instance.CurrentEditorLevel.UnsavedChanges = false;

        Logger.Log(Logger.Datawriting, "Overworld {0} Saved.", _overworldName);
    }

    private void SaveOverworldData()
    {
        OverworldData overworldData = new OverworldData(OverworldGameplayManager.Instance.EditorOverworld).WithName(_overworldName);
        JsonOverworldFileWriter fileWriter = new JsonOverworldFileWriter();
        fileWriter.SerialiseData(overworldData);
    }

    private void AddOverworldToOverworldList()
    {
        OverworldNamesData overworldNameData = new OverworldNamesData(_overworldName).AddOverworldName(_overworldName);

        JsonOverworldListFileWriter fileWriter = new JsonOverworldListFileWriter();
        fileWriter.SerialiseData(overworldNameData);
    }
}
