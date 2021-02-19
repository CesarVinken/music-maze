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
        JsonOverworldListFileReader overworldListFileReader = new JsonOverworldListFileReader();
        OverworldNamesData oldData = overworldListFileReader.ReadOverworldList();

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
