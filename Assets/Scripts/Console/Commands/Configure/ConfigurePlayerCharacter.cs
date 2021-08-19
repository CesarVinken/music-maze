using System.Collections.Generic;
using System.Linq;

public class ConfigurePlayerCharacter : IConfigureCommand
{
    public void Configure(List<string> arguments)
    {
        try
        {
            string message = "";

            if (arguments.Count < 2)
            {
                message = $"<color={ConsoleConfiguration.ErrorColour}>The command '</color><color={ConsoleConfiguration.HighlightColour}>configure player-character</color><color={ConsoleConfiguration.ErrorColour}>' needs additional arguments.</color>\nMake sure that the 3rd argument has the number or name of the player you want to configure, and that the 4th argument contains the player's new score.\n For example 'configure score 1 200'.\n";                
                Logger.Warning(message);

                throw new NotEnoughArgumentsConsoleException(message);
            }

            PlayerNumber playerNumber = TryGetPlayerFromArgument(arguments[0]);
            string sanatisedCharacterName = FirstCharToUpper(arguments[1]);
            
            PersistentGameManager.PlayerCharacterNames[playerNumber] = sanatisedCharacterName;

            ReplacePlayerCharacter(playerNumber, sanatisedCharacterName);

            message = $"<color={ConsoleConfiguration.ErrorColour}>{playerNumber}</color> is now the character <color={ConsoleConfiguration.HighlightColour}>{sanatisedCharacterName}</color><color={ConsoleConfiguration.ErrorColour}>{playerNumber}.</color>";
            Console.Instance.PrintToReportText(message);
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public void ReplacePlayerCharacter(PlayerNumber playerNumber, string characterName)
    {
        try
        {
            // parse requested character
            PlayerCharacter currentPlayerCharacter = GameManager.Instance.CharacterManager.GetPlayerCharacter<PlayerCharacter>(playerNumber);
            GridLocation currentGridLocation = currentPlayerCharacter.CurrentGridLocation;

            ICharacter character = CharacterSpawner.GetCharacterToSpawn(characterName);

            if (character == null)
            {
                string message = $"<color={ConsoleConfiguration.HighlightColour}>Could not find a blueprint with the name {characterName}";
                throw new BlueprintNotFoundConsoleException(message);
            }
            
            CharacterBlueprint blueprint = new CharacterBlueprint(character);

            // remove old character
            GameManager.Instance.CharacterManager.RemovePlayer(playerNumber);

            Logger.Log(Logger.Initialisation, $"Instantiating '{characterName}' for {playerNumber}. Blueprint {blueprint.CharacterType.GetType()}"); // TODO: sanatising character name
            PlayerCharacter player = GameManager.Instance.CharacterManager.SpawnPlayerCharacter(blueprint, currentGridLocation, playerNumber);
        
            if(GameRules.GamePlayerType == GamePlayerType.SplitScreenMultiplayer &&
                playerNumber == PlayerNumber.Player2)
            {
                CameraManager.Instance.CameraControllers[1].SetPlayerTransform(player);
            }
            else
            {
                CameraManager.Instance.CameraControllers[0].SetPlayerTransform(player);
            }

            player.UnfreezeCharacter();
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public PlayerNumber TryGetPlayerFromArgument(string argument)
    {
        string sanatisedArgument = argument.ToLower().Replace(" ", "");
        Dictionary<PlayerNumber, string> players = GameManager.Instance.CharacterManager.GetPlayerNames();

        if (sanatisedArgument.Equals("player1"))
        {
            return PlayerNumber.Player1;
        }
        else if (sanatisedArgument.Equals("player2") && players.Count == 2)
        {
            return PlayerNumber.Player2;
        }

        foreach (KeyValuePair<PlayerNumber, string> item in players)
        {
            string playerName = item.Value;
            if (playerName.ToLower().Equals(argument))
            {
                return item.Key;
            }
        }

        string message = $"<color={ConsoleConfiguration.ErrorColour}>Could not find a player based on the name {argument}</color>";
        throw new CouldNotFindPlayerConsoleException(message);
    }

    private string FirstCharToUpper(string input) =>
    input.First().ToString().ToUpper() + input.Substring(1);
}
