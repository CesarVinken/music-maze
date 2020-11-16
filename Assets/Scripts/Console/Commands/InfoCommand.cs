using System.Collections.Generic;

public class InfoCommand : CommandProcedure
{
    public override void Run(List<string> arguments)
    {
        string infoObject = arguments[0];
        arguments.RemoveAt(0);

        switch (infoObject)
        {
            case "maze":
                GetMazeInfo(new MazeLevelInfo(), arguments);
                break;
            default:
                Console.Instance.PrintToReportText("Unknown info command " + infoObject);
                break;
        }

    }

    private void GetMazeInfo(IInfoCommand infoCommand, List<string> arguments)
    {
        string infoMessage = infoCommand.GetInfo(arguments);
        Console.Instance.PrintToReportText(infoMessage);
    }

    public override void Help()
    {
        string printLine = "Request info about things. Currently known keywords are:\n";
        printLine += "-maze\n";
        printLine += "\n";
        Console.Instance.PrintToReportText(printLine);
    }
}
