public class CouldNotFindMazeLevelConsoleException : System.Exception
{
    public CouldNotFindMazeLevelConsoleException(string message)
    {
        Logger.Warning(message);
        Console.Instance.PrintToReportText(message);
    }
}

