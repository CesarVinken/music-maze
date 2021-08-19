using System.IO;
using UnityEngine;

namespace DataSerialisation
{
    public class JsonMazeLevelFileWriter : IJsonFileWriter
    {
        private MazeLevelData _levelData;
        private string _path;

        public void SerialiseData<T>(T levelData)
        {
            Directory.CreateDirectory(Path.Combine(Application.dataPath, "StreamingAssets", "maze"));

            _levelData = levelData as MazeLevelData;
            _path = Path.Combine(Application.dataPath, "StreamingAssets", "maze", _levelData.Name + ".json");

            string jsonDataString = JsonUtility.ToJson(_levelData, true).ToString();

            File.WriteAllText(_path, jsonDataString);
        }

        public static void DeleteFile(string mazeLevelName)
        {
            string path = Path.Combine(Application.dataPath, "StreamingAssets", "maze", mazeLevelName + ".json");

            File.Delete(path);
        }
    }
}