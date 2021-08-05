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

    public string DeleteLevelName(string mazeLevelName)
    {
        MazeLevelNamesData oldData = new JsonMazeLevelListFileReader().ReadData<MazeLevelNamesData>();

        if (oldData != null)
        {
            LevelNames = oldData.LevelNames;
        } else
        {
            return "Could not find maze level names data.\n\n";
        }

        if (!LevelNameExists(mazeLevelName))
        {
            return $"Could not find a maze level with the name '<color={ConsoleConfiguration.HighlightColour}>{mazeLevelName}</color> in the list. Could not delete '<color={ConsoleConfiguration.HighlightColour}>{mazeLevelName}</color> from levels.json.\n\n";
        }

        MazeLevelNameData levelNameData = LevelNames.FirstOrDefault(l => l.LevelName == mazeLevelName);
        LevelNames.Remove(levelNameData);

        JsonMazeLevelListFileWriter fileWriter = new JsonMazeLevelListFileWriter();
        fileWriter.SerialiseData(this);

        return $"Removed maze level name '<color={ConsoleConfiguration.HighlightColour}>{mazeLevelName}</color> from list of mazes.\n\n";
    }

    public static bool LevelNameExists(string levelName, List<MazeLevelNameData> levelNamesData = null)
    {
        if(levelNamesData == null)
        {
            MazeLevelNamesData existingData = new JsonMazeLevelListFileReader().ReadData<MazeLevelNamesData>();
            levelNamesData = existingData.LevelNames;
        }

        MazeLevelNameData levelNameData = levelNamesData.FirstOrDefault(l => l.LevelName == levelName);

        if(levelNameData == null)
        {
            return false;
        }
        return true;
    }
}
