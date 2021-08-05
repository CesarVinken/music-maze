using System.Collections.Generic;

public class DeleteCommand : CommandProcedure
{
    public override void Run(List<string> arguments)
    {
        string infoObject = arguments[0];
        arguments.RemoveAt(0);

        switch (infoObject)
        {
            case "maze":
                Delete(new DeleteMazeLevel(), arguments);
                break;
            case "overworld":
                //Delete(new DeleteOverworld(), arguments);
                break;
            default:
                Console.Instance.PrintToReportText("Unknown delete command " + infoObject);
                break;
        }
    }

    private void Delete(IDeleteCommand deleteCommand, List<string> arguments)
    {
        string resultMessage = deleteCommand.Delete(arguments);
        Console.Instance.PrintToReportText(resultMessage);
    }

    public override void Help()
    {
        string printLine = "The delete command currently works with these keywords:\n";
        printLine += "- maze\n";
        printLine += "- overworld\n";
        printLine += "\n";
        Console.Instance.PrintToReportText(printLine);
    }
}
