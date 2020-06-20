using System;
using System.Collections;
using System.Collections.Generic;
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

        MazeName mazeName;
        if(!Enum.TryParse(arguments[1], true, out mazeName))
        {
            string printLine = "<color=" + ConsoleConfiguration.HighlightColour + ">" + arguments[1] + "</color> is not a known level and cannot be loaded.\n\n";
            printLine += "The Currently available levels are: \n";
            printLine = GetAllLevelNames(printLine);
            Console.Instance.PrintToReportText(printLine);

            return;
        }

        MazeLevelManager.Instance.UnloadLevel();
        MazeLevelManager.Instance.LoadLevel(mazeName);
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
        MazeName[] mazeNames = (MazeName[])Enum.GetValues(typeof(MazeName));

        for (int i = 0; i < mazeNames.Length; i++)
        {
            printLine += "\n   -" + mazeNames[i];
        }

        return printLine;
    }
}
