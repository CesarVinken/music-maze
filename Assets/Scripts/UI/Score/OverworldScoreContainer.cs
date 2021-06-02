using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct OverworldScore
{
    PlayerNumber PlayerNumber;
    int PlayerScore;
    Text TextLabel;

    public OverworldScore(PlayerNumber playerNumber, int playerScore, Text textLabel)
    {
        PlayerNumber = playerNumber;
        PlayerScore = playerScore;
        TextLabel = textLabel;
    }

    public void UpdatePlayerScore(int playerScore)
    {
        PlayerScore = playerScore;
    }

    public Text GetTextLabel()
    {
        return TextLabel;
    }

    public int GetPlayerScore()
    {
        return PlayerScore;
    }
}

public class OverworldScoreContainer : MonoBehaviour
{
    public static OverworldScoreContainer Instance;

    [SerializeField] private Text _player1ScoreLabel;
    [SerializeField] private Text _player2ScoreLabel;

    public Dictionary<PlayerNumber, OverworldScore> PlayerScores = new Dictionary<PlayerNumber, OverworldScore>();

    public void Awake()
    {
        Guard.CheckIsNull(_player1ScoreLabel, "_player1ScoreLabel", gameObject);
        Guard.CheckIsNull(_player2ScoreLabel, "_player2ScoreLabel", gameObject);

        Instance = this;

        if(GameRules.GamePlayerType == GamePlayerType.SinglePlayer)
        {
            _player2ScoreLabel.gameObject.SetActive(false);
        }
    }

    public void UpdateScoreLabel(PlayerNumber playerNumber)
    {
        Dictionary<PlayerNumber, int> playerOveralScores = PersistentGameManager.PlayerOveralScores;
        Dictionary<PlayerNumber, OverworldPlayerCharacter> playerCharacters = GameManager.Instance.CharacterManager.GetPlayers<OverworldPlayerCharacter>();
        if (PlayerScores.ContainsKey(playerNumber))
        {
            if(playerOveralScores.TryGetValue(playerNumber, out int playerScore))
            {
                PlayerScores[playerNumber].UpdatePlayerScore(playerScore);
            }
            else
            {
                PersistentGameManager.PlayerOveralScores.Add(playerNumber, 0);
                PlayerScores[playerNumber].UpdatePlayerScore(0);
            }
        }
        else
        {
            if (playerOveralScores.TryGetValue(playerNumber, out int playerScore))
            {
                Text textLabel = playerNumber == PlayerNumber.Player1 ? _player1ScoreLabel : _player2ScoreLabel;

                OverworldScore overworldScore = new OverworldScore(playerNumber, playerScore, textLabel);
                PlayerScores.Add(playerNumber, overworldScore);
            }
            else
            {
                PersistentGameManager.PlayerOveralScores.Add(playerNumber, 0);
                
                Text textLabel = playerNumber == PlayerNumber.Player1 ? _player1ScoreLabel : _player2ScoreLabel;
                OverworldScore overworldScore = new OverworldScore(playerNumber, 0, textLabel);
                PlayerScores.Add(playerNumber, overworldScore);
            }
        }

        PlayerScores[playerNumber].GetTextLabel().text = $"{playerCharacters[playerNumber].Name}: {PlayerScores[playerNumber].GetPlayerScore()} points";
        PlayerScores[playerNumber].GetTextLabel().gameObject.SetActive(true);
    }
}
