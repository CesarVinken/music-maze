using System.IO;
using UnityEngine;

public class JsonOverworldFileWriter
{
    private OverworldData _overworldData;
    private string _path;

    public void SerialiseData(OverworldData overworldData)
    {
        Directory.CreateDirectory(Path.Combine(Application.dataPath, "StreamingAssets", "overworld"));

        _overworldData = overworldData;
        _path = Path.Combine(Application.dataPath, "StreamingAssets", "overworld/", _overworldData.Name + ".json");

        string jsonDataString = JsonUtility.ToJson(_overworldData, true).ToString();

        File.WriteAllText(_path, jsonDataString);
    }
}
