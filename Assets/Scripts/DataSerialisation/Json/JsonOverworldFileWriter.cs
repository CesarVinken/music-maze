using System.IO;
using UnityEngine;

namespace DataSerialisation
{
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

        public static void DeleteFile(string overworldName)
        {
            string path = Path.Combine(Application.dataPath, "StreamingAssets", "overworld", overworldName + ".json");

            File.Delete(path);
        }
    }
}