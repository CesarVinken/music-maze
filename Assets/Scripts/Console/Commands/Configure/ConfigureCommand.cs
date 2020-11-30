using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class ConfigureCommand : CommandProcedure
{
    public override void Run(List<string> arguments)
    {
        try
        {
            string infoObject = arguments[0];
            arguments.RemoveAt(0);

            switch (infoObject)
            {
                case "maze":
                    Configure(new ConfigureMaze(), arguments);
                    break;
                case "default-maze":
                    Configure(new ConfigureDefaultMaze(), arguments);
                    break;
                default:
                    //Console.Instance.PrintToReportText();
                    string message = $"Unknown configure argument '{infoObject}' for the configure command. Try 'maze' or 'default-maze'";
                    throw new UnknownArgumentConsoleException(message);
            }
        }
        catch (System.Exception e)
        {
            Logger.Warning(e.Message);
        }
    }

    private void Configure(IConfigureCommand configureCommand, List<string> arguments)
    {
        configureCommand.Configure(arguments);
    }
 

    public static string GetConfigurableArguments()
    {
        string message = "\nThe Currently available arguments to confure are: \n";
        message += "\n-playable\n";
        return message;
    }

    public override void Help()
    {
        string printLine = "The configure command can be used to configure things such as maze levels.\n";
        printLine += "With 'default-maze' as a second argument, you can set the default level. Input the name of the level as third argument\n";
        printLine += "For this use 'maze' as a second argument and enter as a third argument what should be configured. The configured state, such as 'on' or 'off' should be given as a fourth argument. \n\n";

        Console.Instance.PrintToReportText(printLine);
    }
}
