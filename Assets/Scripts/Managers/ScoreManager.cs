using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct PlayerScore
{
    public int TileMarkScore;
    public int PlayerCaughtScore;

    public int TotalScore;

    public PlayerScore(int tileMarkScore = 0, int playerCaughtScore = 0)
    {
        TileMarkScore = 0;
        PlayerCaughtScore = 0;

        TotalScore = 0;
    }

    public int CountTotal()
    {
        TotalScore = TileMarkScore + PlayerCaughtScore;
        return TotalScore;
    }
}

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    public PlayerScore Player1Score = new PlayerScore();
    public PlayerScore Player2Score = new PlayerScore();

    public const int MarkedTileValue = 10;
    public const int PlayerCaughtPenaltyValue = 10;

    public Dictionary<PlayerNumber, int> PlayerScores = new Dictionary<PlayerNumber, int>();

    public void Awake()
    {
        Instance = this;
    }

    public void Start()
    {
        foreach (KeyValuePair<PlayerNumber, PlayerCharacter> player in CharacterManager.Instance.MazePlayers)
        {
            PlayerScores.Add(player.Key, 0);
        }
    }

    public void CountScore()
    {
        CountTileMarkerScore();
        CountTimesCaughtScore();

        Logger.Log(Logger.Score, "Finished counting score.");

        if (CharacterManager.Instance.MazePlayers.Count == 1)
        {
            Player1Score.CountTotal();
            Logger.Log(Logger.Score, $"Total score Player 1: {Player1Score.TotalScore}");
        }
        else
        {
            Player1Score.CountTotal();
            Player2Score.CountTotal();
            Logger.Log(Logger.Score, $"Total score Player 1: {Player1Score.TotalScore}");
            Logger.Log(Logger.Score, $"Total score Player 2: {Player2Score.TotalScore}");
        }
    }

    public void CountTileMarkerScore()
    {
        List<Tile> markedTiles = MazeLevelManager.Instance.Level.Tiles.Where(t => t.PlayerMark != null).ToList();
        for (int i = 0; i < markedTiles.Count; i++)
        {
            PlayerMark playerMark = markedTiles[i].PlayerMark;
            if(playerMark.Owner == PlayerMarkOwner.Player1)
            {
                Player1Score.TileMarkScore += MarkedTileValue;
            }
            else if (playerMark.Owner == PlayerMarkOwner.Player2)
            {
                Player2Score.TileMarkScore += MarkedTileValue;
            }
        }

        if(CharacterManager.Instance.MazePlayers.Count == 2)
            Logger.Log(Logger.Score, $"Finished counting tile marker scores. Player 1 has {Player1Score.TileMarkScore} points and player 2 has {Player2Score.TileMarkScore} points.");
        else
            Logger.Log(Logger.Score, $"Finished counting tile marker scores. Player 1 has {Player1Score.TileMarkScore} points.");
    }

    public void CountTimesCaughtScore()
    {
        Player1Score.PlayerCaughtScore = CharacterManager.Instance.MazePlayers[PlayerNumber.Player1].TimesCaught * -PlayerCaughtPenaltyValue;

        if (CharacterManager.Instance.MazePlayers.Count == 2)
        {
            Player2Score.PlayerCaughtScore = CharacterManager.Instance.MazePlayers[PlayerNumber.Player2].TimesCaught * -PlayerCaughtPenaltyValue;
            Logger.Log(Logger.Score, $"Finished counting player caught scores. Player 1 has {Player1Score.PlayerCaughtScore} points and player 2 has {Player2Score.PlayerCaughtScore} points.");
        }
        else
        {
            Logger.Log(Logger.Score, $"Finished counting player caught scores. Player 1 has {Player1Score.PlayerCaughtScore} points.");
        }
    }
}
