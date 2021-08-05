using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class OverworldNamesData
{
    public List<OverworldNameData> OverworldNames = new List<OverworldNameData>();

    public OverworldNamesData()
    {

    }

    public OverworldNamesData(string overworldName)
    {
        OverworldNamesData oldData = new JsonOverworldListFileReader().ReadData<OverworldNamesData>();

        if (oldData != null)
        {
            OverworldNames = oldData.OverworldNames;
        }
    }

    public OverworldNamesData AddOverworldName(string overworldName)
    {
        if (OverworldNameExists(overworldName))
        {
            Logger.Log($"An overworld with the name {overworldName} was already registered. Not adding it to the overworld name list");
            return this;
        }

        OverworldNameData overworldNameData = new OverworldNameData(overworldName);
        OverworldNames.Add(overworldNameData);

        return this;
    }

    public string DeleteOverworldName(string overworldName)
    {
        OverworldNamesData oldData = new JsonOverworldListFileReader().ReadData<OverworldNamesData>();

        if (oldData != null)
        {
            OverworldNames = oldData.OverworldNames;
        }
        else
        {
            return "Could not find overworld names data.\n\n";
        }

        if (!OverworldNameExists(overworldName))
        {
            return $"Could not find an overworld with the name '<color={ConsoleConfiguration.HighlightColour}>{overworldName}</color> in the list. Could not delete '<color={ConsoleConfiguration.HighlightColour}>{overworldName}</color> from overworlds.json.\n\n";
        }

        OverworldNameData overworldNameData = OverworldNames.FirstOrDefault(l => l.OverworldName == overworldName);
        OverworldNames.Remove(overworldNameData);

        JsonOverworldListFileWriter fileWriter = new JsonOverworldListFileWriter();
        fileWriter.SerialiseData(this);

        return $"Removed overworld name '<color={ConsoleConfiguration.HighlightColour}>{overworldName}</color> from list of overworld.\n\n";
    }

    public bool OverworldNameExists(string overworldName)
    {
        OverworldNameData overworldNameData = OverworldNames.FirstOrDefault(l => l.OverworldName == overworldName);

        if (overworldNameData == null)
        {
            return false;
        }
        return true;
    }
}
