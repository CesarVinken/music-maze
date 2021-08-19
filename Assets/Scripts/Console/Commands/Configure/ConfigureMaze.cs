using DataSerialisation;
using System.Collections.Generic;
using System.Linq;

namespace Console
{
    public class ConfigureMaze : IConfigureCommand
    {
        public void Configure(List<string> arguments)
        {
            try
            {
                if (arguments.Count < 2)
                {
                    string message = "The command '<color=" + ConsoleConfiguration.HighlightColour + ">configure maze</color>' needs additional arguments. \nMake sure that the 2nd argument has the name of the level that needs to be changed, and the 3rd argument says what needs to be configured.\n For example 'configure maze first-level playable true'.\n";
                    Logger.Warning(message);

                    message += ConfigureCommand.GetConfigurableArguments();
                    throw new NotEnoughArgumentsConsoleException(message);
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
                        message += ConfigureCommand.GetConfigurableArguments();
                        throw new UnknownArgumentConsoleException(message);
                }
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        private void ToggleLevelPlayability(string levelName, List<string> arguments)
        {
            if (arguments.Count < 1)
            {
                string message = "The command '<color=" + ConsoleConfiguration.HighlightColour + ">configure maze playable</color>' needs an additional argument saying 'true' or 'false'.";
                throw new NotEnoughArgumentsConsoleException(message);
            }

            MazeLevelNamesData levelNamesData = new JsonMazeLevelListFileReader().ReadData<MazeLevelNamesData>();

            string sanatisedLevelName = levelName.ToLower().Replace(" ", "-");
            int levelNameIndex = levelNamesData.LevelNames.FindIndex(l => l.LevelName == sanatisedLevelName);
            if (levelNameIndex == -1)
            {
                string message = $"Could not find the maze level '<color={sanatisedLevelName}>info maze</color>' in the list with known maze levels.";
                throw new MazeLevelNameNotFoundConsoleException(message);
            }

            MazeLevelNameData levelNameData = levelNamesData.LevelNames.ElementAt(levelNameIndex);

            if (levelNameData == null)
            {
                string message = $"Could not find the maze level '<color={sanatisedLevelName}>info maze</color>' in the list with known maze levels.";
                throw new MazeLevelNameNotFoundConsoleException(message);
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
                    string message = $"The command '<color={ConsoleConfiguration.HighlightColour}>configure maze {levelName} playable</color>' needs a last argument saying 'true' or 'false'.";
                    throw new UnknownArgumentConsoleException(message);
            }

            levelNamesData.LevelNames[levelNameIndex] = levelNameData;
            JsonMazeLevelListFileWriter jsonMazeLevelListFileWriter = new JsonMazeLevelListFileWriter();
            jsonMazeLevelListFileWriter.SerialiseData(levelNamesData);
        }
    }
}