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
            message = MazeLevelLoader.GetAllLevelNamesForPrint(message);
            Console.Instance.PrintToReportText(message);
            return;
        }

        if (MazeLevelManager.Instance == null)
        {
            Logger.Error("Cannot find MazeLevelManager. Returning.");
            return;
        }

        MazeLevelData mazeLevelData = MazeLevelLoader.LoadMazeLevelData(arguments[1]);

        if (mazeLevelData == null && Console.Instance.ConsoleState != ConsoleState.Closed)
        {
            string printLine = "<color=" + ConsoleConfiguration.HighlightColour + ">" + arguments[1] + "</color> is not a known level and cannot be loaded.\n\n";
            printLine += "The Currently available levels are: \n";
            printLine = MazeLevelLoader.GetAllLevelNamesForPrint(printLine);
            Console.Instance.PrintToReportText(printLine);
        }

        MazeLevelLoader.LoadMazeLevel(mazeLevelData);
    }

    public override void Help()
    {
        string printLine = "To load a level with argument 'maze' and then the name of the level. \n\n";
        printLine += "The Currently available levels are: \n";
        printLine = MazeLevelLoader.GetAllLevelNamesForPrint(printLine);
        Console.Instance.PrintToReportText(printLine);
    }


}
