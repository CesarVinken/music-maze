using System.IO;
using UnityEngine;

public class JsonOverworldFileWriter : IJsonFileWriter
{
    private OverworldData _overworldData;
    private string _path;

    public void SerialiseData<T>(T overworldData)
    {
        Directory.CreateDirectory(Path.Combine(Application.dataPath, "StreamingAssets", "overworld"));

        _overworldData = overworldData as OverworldData;
        _path = Path.Combine(Application.dataPath, "StreamingAssets", "overworld/", _overworldData.Name + ".json");

        string jsonDataString = JsonUtility.ToJson(_overworldData, true).ToString();

        File.WriteAllText(_path, jsonDataString);
    }
}
