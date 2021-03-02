using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class MazeLevelNamesData
{
    public List<MazeLevelNameData> LevelNames = new List<MazeLevelNameData>();

    public MazeLevelNamesData()
    {

    }

    public MazeLevelNamesData(string levelName)
    {
        MazeLevelNamesData oldData = new JsonMazeLevelListFileReader().ReadData<MazeLevelNamesData>();

        if (oldData != null)
        {
            LevelNames = oldData.LevelNames;
        }
    }

    public MazeLevelNamesData AddLevelName(string mazeLevelName)
    {
        if(LevelNameExists(mazeLevelName))
        {
            Logger.Log($"A level with the name {mazeLevelName} was already registered. Not adding it to the maze name list");
            return this;
        }

        MazeLevelNameData mazeLevelNameData = new MazeLevelNameData(mazeLevelName);
        LevelNames.Add(mazeLevelNameData);

        return this;
    }

    public bool LevelNameExists(string levelName)
    {
        MazeLevelNameData levelNameData = LevelNames.FirstOrDefault(l => l.LevelName == levelName);

        if(levelNameData == null)
        {
            return false;
        }
        return true;
    }
}
