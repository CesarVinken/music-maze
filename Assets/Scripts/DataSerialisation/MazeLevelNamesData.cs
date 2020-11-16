using System;
using System.Collections.Generic;

[Serializable]
public class MazeLevelNamesData
{
    public List<MazeLevelNameData> LevelNames = new List<MazeLevelNameData>();

    public MazeLevelNamesData()
    {

    }

    public MazeLevelNamesData(string levelName)
    {
        JsonMazeLevelListFileReader mazeLevelListFileReader = new JsonMazeLevelListFileReader();
        MazeLevelNamesData oldData = mazeLevelListFileReader.ReadMazeLevelList();

        if (oldData != null)
        {
            LevelNames = oldData.LevelNames;
        }
        AddLevelName(levelName);
    }

    public void AddLevelName(string levelName)
    {
        MazeLevelNameData myObject = new MazeLevelNameData(levelName);
        LevelNames.Add(myObject);
    }
}
