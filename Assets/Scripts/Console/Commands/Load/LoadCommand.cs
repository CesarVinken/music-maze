using DataSerialisation;
using Photon.Pun;
using System.Collections.Generic;

namespace Console
{
    public class LoadCommand : CommandProcedure
    {
        public override void Run(List<string> arguments)
        {
            switch (arguments[0])
            {
                case "maze":
                    LoadMazeLevel(arguments);
                    break;
                case "overworld":
                    LoadOverworld(arguments);
                    break;
                default:
                    string message = $"Unknown build command to build {arguments[0]}.";
                    throw new UnknownArgumentConsoleException(message);
            }
        }

        public void LoadMazeLevel(List<string> arguments)
        {
            if (MazeLevelGameplayManager.Instance == null)
            {
                if (PersistentGameManager.CurrentSceneType == SceneType.Overworld)
                {
                    Logger.Warning("We are currently in the overworld scene. Switching scenes.");
                    PersistentGameManager.SetLastMazeLevelName("default");
                    PersistentGameManager.SetCurrentSceneName("default");

                    PhotonNetwork.LoadLevel("Maze"); // TODO this loads the default maze, should load specific maze
                }
                else
                {
                    Logger.Error("Cannot find MazeLevelManager. Returning.");

                }
                return;
            }

            MazeLevelData mazeLevelData;

            if (arguments.Count < 2)
            {
                PersistentGameManager.SetLastMazeLevelName("default");
                PersistentGameManager.SetCurrentSceneName("default");

                mazeLevelData = MazeLevelLoader.LoadMazeLevelData(PersistentGameManager.CurrentSceneName);
                MazeLevelLoader.LoadMazeLevel(mazeLevelData);
                return;
            }

            string mazeName = arguments[1];
            mazeLevelData = MazeLevelLoader.LoadMazeLevelData(mazeName);

            if (mazeLevelData == null && Console.Instance.ConsoleState != ConsoleState.Closed)
            {
                string printLine = "<color=" + ConsoleConfiguration.HighlightColour + ">" + arguments[1] + "</color> is not a known maze level and cannot be loaded.\n\n";
                printLine += "The Currently available levels are: \n";
                printLine = MazeLevelLoader.GetAllMazeLevelNamesForPrint(printLine);
                Console.Instance.PrintToReportText(printLine);
            }

            PersistentGameManager.SetLastMazeLevelName(mazeName);
            PersistentGameManager.SetCurrentSceneName(mazeName);

            mazeLevelData = MazeLevelLoader.LoadMazeLevelData(PersistentGameManager.CurrentSceneName);
            MazeLevelLoader.LoadMazeLevel(mazeLevelData);
        }

        public void LoadOverworld(List<string> arguments)
        {
            Logger.Warning("TODO: Load overworld based on arguments");
            /*
            if (OverworldManager.Instance == null)
            {
                if (GameManager.CurrentSceneType == SceneType.Maze)
                {
                    Logger.Warning("We are currently in the maze scene. Switching scenes.");
                    SceneManager.LoadScene("Overworld"); // TODO this loads the default overworld, should load specific overworld
                }
                else
                {
                Logger.Error("Cannot find OverworldManager. Returning.");

                }
                return;
            }

            OverworldData overworldData;

            if (arguments.Count < 2)
            {
                overworldData = OverworldLoader.LoadOverworldData("default");
                OverworldLoader.LoadOverworld(overworldData);
                return;
            }
            overworldData = OverworldLoader.LoadMazeLevelData(arguments[1]);

            if (overworldData == null && Console.Instance.ConsoleState != ConsoleState.Closed)
            {
                string printLine = "<color=" + ConsoleConfiguration.HighlightColour + ">" + arguments[1] + "</color> is not a known overworld and cannot be loaded.\n\n";
                printLine += "The Currently available overworlds are: \n";
                printLine = OverworldLoader.GetAllOverworldsNamesForPrint(printLine);
                Console.Instance.PrintToReportText(printLine);
            }

            OverworldLoader.LoadOverworld();
            */
        }

        public override void Help()
        {
            string printLine = "To load a maze level with argument 'maze' and then the name of the level. Without extra maze name argument, 'maze' will load the default maze level. \n\n";
            printLine += "The Currently available levels are: \n";
            printLine = MazeLevelLoader.GetAllMazeLevelNamesForPrint(printLine);
            printLine += "To load the overworld use the argument 'overworld', with optionally an additional argument for the name of a specific overworld version. Without extra overworld name, the default overworld is loaded. \n\n";
            Console.Instance.PrintToReportText(printLine);
        }
    }
}