namespace Console
{
    public class MazeLevelNameNotFoundConsoleException : System.Exception
    {
        public MazeLevelNameNotFoundConsoleException(string message)
        {
            Logger.Warning(message);
            Console.Instance.PrintToReportText(message);
        }
    }
}