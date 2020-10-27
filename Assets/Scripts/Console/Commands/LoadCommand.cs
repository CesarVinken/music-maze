using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LoadCommand : CommandProcedure
{
    public override void Run(List<string> arguments)
    {
        switch (arguments[0])
        {
            case "maze":
                LoadMazeLevel(arguments);
                break;
            default:
                Console.Instance.PrintToReportText("Unknown build command to build " + arguments[0]);
                break;
        }
    }

    public void LoadMazeLevel(List<string> arguments)
    {
        if (arguments.Count < 2)
        {
            string message = "The command '<color=" + ConsoleConfiguration.HighlightColour + ">load maze</color>' needs an additional argument with the name of the maze level";
            Logger.Warning(message);

            message += "\nThe Currently available levels are: \n";
            message = GetAllLevelNames(message);
            Console.Instance.PrintToReportText(message);
            return;
        }

        if (MazeLevelManager.Instance == null)
        {
            Logger.Error("Cannot find MazeLevelManager. Returning.");
            return;
        }

        JsonMazeLevelFileReader levelReader = new JsonMazeLevelFileReader();
        MazeLevelData levelData = levelReader.LoadLevel(arguments[1]);

        if(levelData == null)
        {
            string printLine = "<color=" + ConsoleConfiguration.HighlightColour + ">" + arguments[1] + "</color> is not a known level and cannot be loaded.\n\n";
            printLine += "The Currently available levels are: \n";
            printLine = GetAllLevelNames(printLine);
            Console.Instance.PrintToReportText(printLine);
        }

        // Make checks such as if there are starting locations for the players

        MazeLevelManager.Instance.UnloadLevel();
        MazeLevelManager.Instance.LoadLevel(levelData);
    }

    public override void Help()
    {
        string printLine = "To load a level with argument 'maze' and then the name of the level. \n\n";
        printLine += "The Currently available levels are: \n";
        printLine = GetAllLevelNames(printLine);
        Console.Instance.PrintToReportText(printLine);
    }

    public string GetAllLevelNames(string printLine)
    {
        foreach (string mazeName in Directory.GetFiles(Application.streamingAssetsPath, "*.json"))
        {
            string[] fileNameParts = mazeName.Split('\\');
            string[] fileNameWithoutExtention = fileNameParts[fileNameParts.Length - 1].Split('.');
            printLine += "\n   -" + fileNameWithoutExtention[0];
        }

        return printLine;
    }
}
