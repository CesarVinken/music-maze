namespace Console
{
    public class OverworldNameNotFoundConsoleException : System.Exception
    {
        public OverworldNameNotFoundConsoleException(string message)
        {
            Logger.Warning(message);
            Console.Instance.PrintToReportText(message);
        }
    }
}