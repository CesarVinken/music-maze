using DataSerialisation;
using System.Collections.Generic;
using System.Linq;

namespace Console
{
    public class OverworldListInfo : IInfoCommand
    {
        public string GetInfo(List<string> arguments)
        {
            try
            {
                List<string> overworldNames = GetOverworldNamesData();
                Logger.Log($"overworldNames has a count of {overworldNames.Count}");
                string infoMessage = "--\n";
                infoMessage += $"There are {overworldNames.Count} overworlds in total:\n\n";

                for (int i = 0; i < overworldNames.Count; i++)
                {
                    infoMessage += $"- {overworldNames[i]}\n";
                }

                infoMessage += "\n\n";

                return infoMessage;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        private List<string> GetOverworldNamesData()
        {
            OverworldNamesData overworldNamesData = OverworldLoader.GetAllOverworldNamesData();
            Logger.Log($"overworldNamesData has a count of {overworldNamesData.OverworldNames.Count}");
            List<string> overworldNames = overworldNamesData.OverworldNames.Select(level => level.OverworldName).ToList();

            return overworldNames;
        }
    }
}