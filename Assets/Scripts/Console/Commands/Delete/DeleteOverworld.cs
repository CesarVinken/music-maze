using DataSerialisation;
using System.Collections.Generic;

namespace Console
{
    public class DeleteOverworld : IDeleteCommand
    {
        public string Delete(List<string> arguments)
        {
            try
            {
                if (arguments.Count < 1)
                {
                    string message = "The command '<color=" + ConsoleConfiguration.HighlightColour + ">delete overworld</color>' needs an additional argument with the name of the overworld that you want to delete.\n";
                    Logger.Warning(message);

                    throw new NotEnoughArgumentsConsoleException(message);
                }

                string sanatisedOverworldName = arguments[0].ToLower().Replace(" ", "-");

                string infoString = "\n";

                infoString += DeleteOverworldFile(sanatisedOverworldName);
                infoString += DeleteOverworldFromOverworldNamesList(sanatisedOverworldName);
                infoString += $"Deletion of overworld '<color={ConsoleConfiguration.HighlightColour}>{sanatisedOverworldName}</color> completed.'\n\n";

                return infoString;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        private string DeleteOverworldFile(string sanatisedOverworldName)
        {
            bool overworldExists = OverworldLoader.OverworldExists(sanatisedOverworldName);

            if (!overworldExists)
            {
                string message = $"Could not find an overworld with the name '<color={ConsoleConfiguration.HighlightColour}>{sanatisedOverworldName}</color>'.\n";
                throw new OverworldNameNotFoundConsoleException(message);
            }

            JsonOverworldFileWriter.DeleteFile(sanatisedOverworldName);

            return $"Deleted overworld data file for '<color={ConsoleConfiguration.HighlightColour}>{sanatisedOverworldName}</color>.\n\n";
        }

        private string DeleteOverworldFromOverworldNamesList(string sanatisedOverworldName)
        {
            OverworldNamesData overworldNamesData = new OverworldNamesData(sanatisedOverworldName);
            string infoString = overworldNamesData.DeleteOverworldName(sanatisedOverworldName);

            return infoString;
        }
    }
}