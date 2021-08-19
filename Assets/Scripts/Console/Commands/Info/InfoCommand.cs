using System.Collections.Generic;

namespace Console
{
    public class InfoCommand : CommandProcedure
    {
        public override void Run(List<string> arguments)
        {
            string infoObject = arguments[0];
            arguments.RemoveAt(0);

            switch (infoObject)
            {
                case "maze":
                    GetInfo(new MazeLevelInfo(), arguments);
                    break;
                case "maze-list":
                    GetInfo(new MazeLevelListInfo(), arguments);
                    break;
                case "overworld":
                    GetInfo(new OverworldInfo(), arguments);
                    break;
                case "overworld-list":
                    GetInfo(new OverworldListInfo(), arguments);
                    break;
                default:
                    Console.Instance.PrintToReportText("Unknown info command " + infoObject);
                    break;
            }
        }

        private void GetInfo(IInfoCommand infoCommand, List<string> arguments)
        {
            string infoMessage = infoCommand.GetInfo(arguments);
            Console.Instance.PrintToReportText(infoMessage);
        }

        public override void Help()
        {
            string printLine = "Request info about things. Currently known keywords are:\n";
            printLine += "- maze\n";
            printLine += "- maze-list\n";
            printLine += "- overworld\n";
            printLine += "- overworld-list\n";
            printLine += "\n";
            Console.Instance.PrintToReportText(printLine);
        }
    }
}