using System.IO;
using UnityEngine;

public class JsonOverworldListFileWriter : IJsonFileWriter
{
    private OverworldNamesData _overworldNamesData;

    private string _path;

    public void SerialiseData<T>(T levelNamesData)
    {
        _overworldNamesData = levelNamesData as OverworldNamesData;
        _path = Path.Combine(Application.dataPath, "StreamingAssets", "overworld", "overworlds.json");

        string jsonOverworldNamesData = JsonUtility.ToJson(_overworldNamesData, true).ToString();

        File.WriteAllText(_path, jsonOverworldNamesData);
        Logger.Log(Logger.Datawriting, "Wrote overworlds list to file");
    }
}
