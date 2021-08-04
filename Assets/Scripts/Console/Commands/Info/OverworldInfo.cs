using System.Collections.Generic;
using System.Linq;

public class OverworldInfo : IInfoCommand
{
    public string GetInfo(List<string> arguments)
    {
        try
        {
            if (arguments.Count < 1)
            {
                string message = "The command '<color=" + ConsoleConfiguration.HighlightColour + ">info overworld</color>' needs an additional argument with the name of the overworld you want info on.";
                Logger.Warning(message);

                throw new NotEnoughArgumentsConsoleException(message);
            }

            string sanatisedOverworldName = arguments[0].ToLower().Replace(" ", "-");

            OverworldNameData overworldName = GetOverworldNameData(sanatisedOverworldName);
            OverworldData overworldData = GetOverworldData(sanatisedOverworldName);

            bool isPlayable = GetIsPlayable(overworldName);
            GridLocation overworldBounds = GetOverworldBounds(overworldData);

            string infoMessage = "--\n";
            infoMessage += $"Information for overworld {arguments[0]}\n";
            infoMessage += "--\n\n";
            infoMessage += $"Name: {sanatisedOverworldName}\n";
            infoMessage += $"Playable: {isPlayable}\n";
            infoMessage += $"Rows: {overworldBounds.X + 1}\n";
            infoMessage += $"Columns: {overworldBounds.Y + 1}\n";
            infoMessage += "\n\n";

            return infoMessage;
        }
        catch (System.Exception)
        {
            return null;
        }
    }

    private OverworldData GetOverworldData(string sanatisedOverworldName)
    {
        bool overworldExists = OverworldLoader.OverworldExists(sanatisedOverworldName);

        if (!overworldExists)
        {
            string message = $"Could not find an overworld with the name '<color={ConsoleConfiguration.HighlightColour}>{sanatisedOverworldName}</color>'.\n";
            throw new OverworldNameNotFoundConsoleException(message);
        }

        OverworldData overworldData = OverworldLoader.LoadOverworldData(sanatisedOverworldName);
        return overworldData;
    }

    private OverworldNameData GetOverworldNameData(string sanatisedOverworldName)
    {
        OverworldNamesData overworldNamesData = OverworldLoader.GetAllOverworldNamesData();
        OverworldNameData overworldName = overworldNamesData.OverworldNames.FirstOrDefault(level => level.OverworldName == sanatisedOverworldName);

        if (overworldName == null)
        {
            string message = $"Could not find an overworld with the name '<color={ConsoleConfiguration.HighlightColour}>{sanatisedOverworldName}</color>' in the overworlds list.\n";
            throw new OverworldNameNotFoundConsoleException(message);
        }

        return overworldName;
    }

    private bool GetIsPlayable(OverworldNameData overworldName)
    {
        bool isPlayable = overworldName.IsPlayable;
        return isPlayable;
    }

    private GridLocation GetOverworldBounds(OverworldData overworldData)
    {
        GridLocation furthestBounds = new GridLocation(0, 0);

        for (int i = 0; i < overworldData.Tiles.Count; i++)
        {
            SerialisableTile tile = overworldData.Tiles[i];
            if (tile.GridLocation.X > furthestBounds.X) furthestBounds.X = tile.GridLocation.X;
            if (tile.GridLocation.Y > furthestBounds.Y) furthestBounds.Y = tile.GridLocation.Y;
        }

        return furthestBounds;
    }
}
