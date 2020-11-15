using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ConfigureCommand : CommandProcedure
{
    public override void Run(List<string> arguments)
    {
        switch (arguments[0])
        {
            case "maze":
                arguments.RemoveAt(0);
                ConfigureMazeLevel(arguments);
                break;
            default:
                Console.Instance.PrintToReportText("Unknown configure command " + arguments[0]);
                break;
        }
    }

    private void ConfigureMazeLevel(List<string> arguments)
    {
        if (arguments.Count < 2)
        {
            string message = "The command '<color=" + ConsoleConfiguration.HighlightColour + ">configure maze</color>' needs additional arguments. \nMake sure that the 2nd argument has the name of the level that needs to be changed, and the 3rd argument says what needs to be configured.\n For example 'configure maze first-level playable true'.\n";
            Logger.Warning(message);

            message += GetConfigurableArguments();
            Console.Instance.PrintToReportText(message);
            return;
        }

        string levelName = arguments[0];

        switch (arguments[1])
        {
            case "playable":
                arguments.RemoveAt(1);
                arguments.RemoveAt(0);
                ToggleLevelPlayability(levelName, arguments);
                break;
            default:
                string message = $"{arguments[1]} Is an unknown argument to configure.";
                message += GetConfigurableArguments();
                Console.Instance.PrintToReportText(message);
                return;
        }
    }

    private void ToggleLevelPlayability(string levelName, List<string> arguments)
    {
        if (arguments.Count < 1)
        {
            string message = "The command '<color=" + ConsoleConfiguration.HighlightColour + ">configure maze playable</color>' needs an additional argument saying 'true' or 'false'.";
            Console.Instance.PrintToReportText(message);
            return;
        }

        JsonMazeLevelListFileReader jsonMazeLevelListFileReader = new JsonMazeLevelListFileReader();
        LevelNamesData levelNamesData = jsonMazeLevelListFileReader.ReadMazeLevelList();

        string sanatisedLevelName = levelName.ToLower().Replace(" ", "-");
        int levelNameIndex = levelNamesData.LevelNames.FindIndex(l => l.LevelName == sanatisedLevelName);
        if (levelNameIndex == -1)
        {
            string message = $"Could not find the maze level {sanatisedLevelName} in the list with known maze levels.";
            Console.Instance.PrintToReportText(message);
            return;
        }
        
        LevelNameData levelNameData = levelNamesData.LevelNames.ElementAt(levelNameIndex);

        if (levelNameData == null)
        {
            string message = $"Could not find the maze level {sanatisedLevelName} in the list with known maze levels.";
            Console.Instance.PrintToReportText(message);
            return;
        }

        switch (arguments[0])
        {
            case "true":
                levelNameData.IsPlayable = true;
                break;
            case "false":
                levelNameData.IsPlayable = false;
                break;
            default:
                string message = $"The command '<color={ConsoleConfiguration.HighlightColour}>configure maze {levelName} playable</color>' needs an additional argument saying 'true' or 'false'.";
                Console.Instance.PrintToReportText(message);
                return;
        }

        levelNamesData.LevelNames[levelNameIndex] = levelNameData;
        JsonMazeLevelListFileWriter jsonMazeLevelListFileWriter = new JsonMazeLevelListFileWriter();
        jsonMazeLevelListFileWriter.SerialiseData(levelNamesData);
    }

    private string GetConfigurableArguments()
    {
        string message = "\nThe Currently available arguments to confure are: \n";
        message += "\n-playable\n";
        return message;
    }

    public override void Help()
    {
        string printLine = "The configure command can be used to configure things such as maze levels. For this use 'maze' as a second argument and enter as a third argument what should be configured. The configured state, such as 'on' or 'off' should be given as a fourth argument. \n\n";
        Console.Instance.PrintToReportText(printLine);
    }
}
