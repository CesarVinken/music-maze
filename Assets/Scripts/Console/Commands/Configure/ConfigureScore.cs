using System.Collections.Generic;

public class ConfigureScore : IConfigureCommand
{
    public void Configure(List<string> arguments)
    {
        try
        {
            string message = "";

            if (arguments.Count < 2)
            {
                message = $"<color={ConsoleConfiguration.ErrorColour}>The command '</color><color={ConsoleConfiguration.HighlightColour}>configure score</color><color={ConsoleConfiguration.ErrorColour}>' needs additional arguments.</color>\nMake sure that the 3rd argument has the number or name of the player you want to configure, and that the 4th argument contains the player's new score.\n For example 'configure score 1 200'.\n";
                Logger.Warning(message);

                throw new NotEnoughArgumentsConsoleException(message);
            }

            PlayerNumber playerNumber = TryGetPlayerFromArgument(arguments[0]);
            if(!int.TryParse(arguments[1], out int newScore))
            {
                message = $"<color={ConsoleConfiguration.ErrorColour}>Could not parse {arguments[1]} as a valid number of points. Make sure to input a number without decimals.</color>";
                throw new UnknownArgumentConsoleException(message);
            }

            PersistentGameManager.PlayerOveralScores[playerNumber] = newScore;

            if(PersistentGameManager.CurrentSceneType == SceneType.Overworld)
            {
                OverworldScoreContainer.Instance.UpdateScoreLabel(playerNumber);
            }

            message = $"<color={ConsoleConfiguration.HighlightColour}>{ playerNumber}</color> now as {PersistentGameManager.PlayerOveralScores[playerNumber]} points.";
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
        else if(sanatisedArgument.Equals("player2") && players.Count == 2)
        {
            return PlayerNumber.Player2;
        }

        foreach (KeyValuePair<PlayerNumber, string> item in players)
        {
            string playerName = item.Value;
            if(playerName.ToLower().Equals(argument))
            {
                return item.Key;
            }
        }

        string message = $"<color={ConsoleConfiguration.ErrorColour}>Could not find a player based on the name {argument}</color>";
        throw new CouldNotFindPlayerConsoleException(message);
    }
}
