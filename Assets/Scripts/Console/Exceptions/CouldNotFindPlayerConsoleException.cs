namespace Console
{
    public class CouldNotFindPlayerConsoleException : System.Exception
    {
        public CouldNotFindPlayerConsoleException(string message)
        {
            Logger.Warning(message);
            Console.Instance.PrintToReportText(message);
        }
    }
}