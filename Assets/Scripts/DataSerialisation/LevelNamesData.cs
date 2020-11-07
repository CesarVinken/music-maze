using System;
using System.Collections.Generic;

[Serializable]
public class LevelNamesData
{
    public List<LevelNameData> LevelNames = new List<LevelNameData>();

    public LevelNamesData()
    {

    }

    public LevelNamesData(string levelName)
    {
        JsonMazeLevelListFileReader mazeLevelListFileReader = new JsonMazeLevelListFileReader();
        LevelNamesData oldData = mazeLevelListFileReader.ReadMazeLevelList();

        if (oldData != null)
        {
            LevelNames = oldData.LevelNames;
        }
        AddLevelName(levelName);
    }

    public void AddLevelName(string levelName)
    {
        LevelNameData myObject = new LevelNameData(levelName);
        LevelNames.Add(myObject);
    }
}
