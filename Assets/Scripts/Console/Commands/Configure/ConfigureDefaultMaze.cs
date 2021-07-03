using System.Collections.Generic;

public class ConfigureDefaultMaze : IConfigureCommand
{
    public void Configure(List<string> arguments)
    {
        try
        {
            if (arguments.Count < 1)
            {
                string message = "The command '<color=" + ConsoleConfiguration.HighlightColour + ">configure default-maze</color>' needs an additional argument with the name of the level that should be the new default.json";

                message += ConfigureCommand.GetConfigurableArguments();
                throw new NotEnoughArgumentsConsoleException(message);
            }

            string sanatisedLevelName = arguments[0].ToLower().Replace(" ", "-");

            bool levelExists = MazeLevelLoader.MazeLevelExists(sanatisedLevelName);

            if (!levelExists)
            {
                string message = $"Could not find a maze level with the name {sanatisedLevelName}.";
                throw new CouldNotFindMazeLevelConsoleException(message);
            }

            MazeLevelLoader.ReplaceMazeLevel(sanatisedLevelName, "default");

            Console.Instance.PrintToReportText($"{sanatisedLevelName} is now the default maze level.");
        }
        catch (System.Exception)
        {
            throw;
        }
        
    }
}
