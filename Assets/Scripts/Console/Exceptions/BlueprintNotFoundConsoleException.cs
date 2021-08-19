namespace Console
{
    public class BlueprintNotFoundConsoleException : System.Exception
    {
        public BlueprintNotFoundConsoleException(string message)
        {
            Logger.Warning(message);
            Console.Instance.PrintToReportText(message);
        }
    }
}