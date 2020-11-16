using System.Collections.Generic;
using System.Linq;

public class MazeLevelInfo : IInfoCommand
{
    public string GetInfo(List<string> arguments)
    {
        try
        {
            if (arguments.Count < 1)
            {
                string message = "The command '<color=" + ConsoleConfiguration.HighlightColour + ">info maze</color>' needs an additional argument with the name of the maze level you want info on.";
                Logger.Warning(message);

                Console.Instance.PrintToReportText(message);
                throw new System.Exception();
            }

            string sanatisedLevelName = arguments[0].ToLower().Replace(" ", "-");

            MazeLevelData mazeLevelData = GetMazeLevelData(sanatisedLevelName);
            MazeLevelNameData mazeLevelName = GetMazeLevelNameData(sanatisedLevelName);

            bool isPlayable = GetIsPlayable(mazeLevelName);
            GridLocation mazeLevelBounds = GetMazeLevelBounds(mazeLevelData);

            string infoMessage = "--\n";
            infoMessage += $"Information for maze level {arguments[0]}\n";
            infoMessage += "--\n\n";
            infoMessage += $"Name: {sanatisedLevelName}\n";
            infoMessage += $"Playable: {isPlayable}\n";
            infoMessage += $"Rows: {mazeLevelBounds.X + 1}\n";
            infoMessage += $"Columns: {mazeLevelBounds.Y + 1}\n";
            infoMessage += "\n\n";

            return infoMessage;
        }
        catch (System.Exception)
        {
            return null;       
        }
    }

    private MazeLevelData GetMazeLevelData(string sanatisedLevelName)
    {
        bool levelExists = MazeLevelLoader.MazeLevelExists(sanatisedLevelName);

        if (!levelExists)
        {
            string message = $"Could not find a maze level with the name {sanatisedLevelName}.\n";
            Console.Instance.PrintToReportText(message);
            throw new System.Exception();
        }

        MazeLevelData mazeLevelData = MazeLevelLoader.LoadMazeLevelData(sanatisedLevelName);
        return mazeLevelData;
    }

    private MazeLevelNameData GetMazeLevelNameData(string sanatisedLevelName)
    {
        MazeLevelNamesData mazeLevelNamesData = MazeLevelLoader.GetAllLevelNamesData();
        MazeLevelNameData mazeLevelName = mazeLevelNamesData.LevelNames.FirstOrDefault(level => level.LevelName == sanatisedLevelName);

        if (mazeLevelName == null)
        {
            string message = $"Could not find a maze level with the name {sanatisedLevelName} in the level list.\n";
            Console.Instance.PrintToReportText(message);
            throw new System.Exception();
        }

        return mazeLevelName;
    }

    private bool GetIsPlayable(MazeLevelNameData mazeLevelName)
    {
        bool isPlayable = mazeLevelName.IsPlayable;
        return isPlayable;
    }

    private GridLocation GetMazeLevelBounds(MazeLevelData mazeLevelData)
    {
        GridLocation furthestBounds = new GridLocation(0, 0);

        for (int i = 0; i < mazeLevelData.Tiles.Count; i++)
        {
            SerialisableTile tile = mazeLevelData.Tiles[i];
            if (tile.GridLocationX > furthestBounds.X) furthestBounds.X = tile.GridLocationX;
            if (tile.GridLocationY > furthestBounds.Y) furthestBounds.Y = tile.GridLocationY;
        }

        return furthestBounds;
    } 
}
