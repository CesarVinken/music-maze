using System.Collections.Generic;

public class DeleteMazeLevel : IDeleteCommand
{
    public string Delete(List<string> arguments)
    {
        try
        {
            if (arguments.Count < 1)
            {
                string message = "The command '<color=" + ConsoleConfiguration.HighlightColour + ">delete maze</color>' needs an additional argument with the name of the maze level that you want to delete.\n";
                Logger.Warning(message);

                throw new NotEnoughArgumentsConsoleException(message);
            }

            string sanatisedLevelName = arguments[0].ToLower().Replace(" ", "-");

            string infoString = "\n";

            infoString += DeleteMazeLevelFile(sanatisedLevelName);
            infoString += DeleteMazeLevelFromMazeNamesList(sanatisedLevelName);
            infoString += $"Deletion of maze '<color={ConsoleConfiguration.HighlightColour}>{sanatisedLevelName}</color> completed.'\n\n";

            return infoString;
        }
        catch (System.Exception)
        {
            return null;
        }
    }

    private string DeleteMazeLevelFile(string sanatisedLevelName)
    {
        bool levelExists = MazeLevelLoader.MazeLevelExists(sanatisedLevelName);

        if (!levelExists)
        {
            string message = $"Could not find a maze level with the name '<color={ConsoleConfiguration.HighlightColour}>{sanatisedLevelName}</color>'.\n";
            throw new MazeLevelNameNotFoundConsoleException(message);
        }

        JsonMazeLevelFileWriter.DeleteFile(sanatisedLevelName);

        return $"Deleted maze level data file for '<color={ConsoleConfiguration.HighlightColour}>{sanatisedLevelName}</color>.\n\n";
    }

    private string DeleteMazeLevelFromMazeNamesList(string sanatisedLevelName)
    {
        MazeLevelNamesData levelNamesData = new MazeLevelNamesData(sanatisedLevelName);
        string infoString = levelNamesData.DeleteLevelName(sanatisedLevelName);

        return infoString;
    }
}
