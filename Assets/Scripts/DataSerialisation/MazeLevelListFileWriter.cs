using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class MazeLevelListFileWriter
{
    private string _existingMazeLevelListData;
    private string _path;

    public void SerialiseData(string levelName)
    {
        _path = Path.Combine(Application.dataPath, "StreamingAssets", "levels.txt");
        MazeLevelListFileReader fileReader = new MazeLevelListFileReader();
        _existingMazeLevelListData = fileReader.ReadMazeLevelList();

        //TODO don't print already existing level

        Logger.Log("levelName:" + levelName);
        string levelList = _existingMazeLevelListData + "*" + levelName + "\n";
        Logger.Log("_existingMazeLevelListData:" + levelList);
        File.WriteAllText(_path, levelList);
    }
}

