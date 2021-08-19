namespace Console
{
    public class UnknownArgumentConsoleException : System.Exception
    {
        public UnknownArgumentConsoleException(string message)
        {
            Logger.Warning(message);
            Console.Instance.PrintToReportText(message);
        }
    }
}