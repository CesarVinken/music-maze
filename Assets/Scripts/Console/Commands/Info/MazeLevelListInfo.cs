using System.Collections.Generic;
using System.Linq;

namespace Console
{
    public class MazeLevelListInfo : IInfoCommand
    {
        public string GetInfo(List<string> arguments)
        {
            try
            {
                List<string> levelNames = GetMazeLevelNamesData();

                string infoMessage = "--\n";
                infoMessage += $"There are {levelNames.Count} maze levels in total:\n\n";

                for (int i = 0; i < levelNames.Count; i++)
                {
                    infoMessage += $"- {levelNames[i]}\n";
                }

                infoMessage += "\n\n";

                return infoMessage;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        private List<string> GetMazeLevelNamesData()
        {
            MazeLevelNamesData mazeLevelNamesData = MazeLevelLoader.GetAllMazeLevelNamesData();
            List<string> mazeLevelNames = mazeLevelNamesData.LevelNames.Select(level => level.LevelName).ToList();

            return mazeLevelNames;
        }
    }
}