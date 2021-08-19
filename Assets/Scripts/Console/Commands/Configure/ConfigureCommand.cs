using System.Collections.Generic;

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
                case "default-maze":
                    Configure(new ConfigureDefaultMaze(), arguments);
                    break;
                case "maze":
                    Configure(new ConfigureMaze(), arguments);
                    break;
                case "player-character":
                    Configure(new ConfigurePlayerCharacter(), arguments);
                    break;
                case "score":
                    Configure(new ConfigureScore(), arguments);
                    break;
                default:
                    //Console.Instance.PrintToReportText();
                    string message = $"Unknown configure argument '{infoObject}' for the configure command. Try 'score', 'maze' or 'default-maze'";
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
        printLine += "With '<color=" + ConsoleConfiguration.HighlightColour + ">default-maze</color>' as a second argument, you can set the default level. Input the name of the level as third argument\n\n";
        printLine += "For this use '<color=" + ConsoleConfiguration.HighlightColour + ">maze</color>' as a second argument and enter as a third argument what should be configured. The configured state, such as 'on' or 'off' should be given as a fourth argument. \n\n";
        printLine += "With '<color=" + ConsoleConfiguration.HighlightColour + ">player-character</color>' as a second argument, you can change which character a player plays with. As second argument enter the PlayerNumber (eg. Player1), and as third argument enter the desired player character (eg. Emmon)\n\n";
        printLine += "With '<color=" + ConsoleConfiguration.HighlightColour + ">score</color>' as a second argument, you can set the score of a player. Input the name of the player as third argument and their desired score as fourth argument\n\n";

        Console.Instance.PrintToReportText(printLine);
    }
}
