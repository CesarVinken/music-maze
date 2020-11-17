public class NotEnoughArgumentsConsoleException : System.Exception
{
    public NotEnoughArgumentsConsoleException(string message)
    {
        Logger.Warning(message);
        Console.Instance.PrintToReportText(message);
    }
}