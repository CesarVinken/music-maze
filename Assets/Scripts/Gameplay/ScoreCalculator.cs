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
        TileMarkScore = tileMarkScore;
        PlayerCaughtScore = playerCaughtScore;

        TotalScore = 0;
    }

    public int CountTotal()
    {
        TotalScore = TileMarkScore + PlayerCaughtScore;
        return TotalScore;
    }

}

public class ScoreCalculator
{
    public const int MarkedTileValue = 10;
    public const int PlayerCaughtPenaltyValue = 10;

    public Dictionary<PlayerNumber, PlayerScore> PlayerScores = new Dictionary<PlayerNumber, PlayerScore>();

    public void CalculateScores()
    {
        Logger.Log(Logger.Score, "Finished counting score.");

        if (CharacterManager.Instance.MazePlayers.Count == 1)
        {
            PlayerScores.Add(PlayerNumber.Player1, new PlayerScore());
        }
        else
        {
            PlayerScores.Add(PlayerNumber.Player1, new PlayerScore());
            PlayerScores.Add(PlayerNumber.Player2, new PlayerScore());
        }

        CountTileMarkerScores();
        CountTimesCaughtScores();

        Dictionary<PlayerNumber, PlayerScore> tempPlayerScores = new Dictionary<PlayerNumber, PlayerScore>();

        foreach (KeyValuePair<PlayerNumber, PlayerScore> item in PlayerScores)
        {
            int total = item.Value.CountTotal();
            PlayerScore p = item.Value;
            p.TotalScore = total;
            tempPlayerScores.Add(item.Key, p);
            Logger.Log(Logger.Score, $"Total score {item.Key.ToString()}: {item.Value.TotalScore}");
        }

        PlayerScores = tempPlayerScores;
    }

    private void CountTileMarkerScores()
    {
        Dictionary<PlayerNumber, int> tempPlayerScores = new Dictionary<PlayerNumber, int>();

        int playerMarkScorePlayer1 = 0;
        int playerMarkScorePlayer2 = 0;
        List<InGameMazeTile> markedTiles = MazeLevelManager.Instance.Level.Tiles.Where(t => t.PlayerMark != null).ToList();
        for (int i = 0; i < markedTiles.Count; i++)
        {
            PlayerMark playerMark = markedTiles[i].PlayerMark;

            if (playerMark.Owner == PlayerMarkOwner.Player1)
            {
                playerMarkScorePlayer1 += MarkedTileValue;
            }
            else if (playerMark.Owner == PlayerMarkOwner.Player2)
            {
                playerMarkScorePlayer2 += MarkedTileValue;
            }
        }

        tempPlayerScores.Add(PlayerNumber.Player1, playerMarkScorePlayer1);
        if (PlayerScores.ContainsKey(PlayerNumber.Player2))
        {
            tempPlayerScores.Add(PlayerNumber.Player2, playerMarkScorePlayer2);
        }

        foreach (KeyValuePair<PlayerNumber, int> item in tempPlayerScores)
        {
            PlayerScore p = PlayerScores[item.Key];
            p.TileMarkScore = item.Value;
            PlayerScores[item.Key] = p;
            Logger.Log(Logger.Score, $"Tile marker scores: {item.Key.ToString()} has {PlayerScores[item.Key].TileMarkScore} points.");
        }
    }

    private void CountTimesCaughtScores()
    {
        Dictionary<PlayerNumber, PlayerScore> tempPlayerScores = new Dictionary<PlayerNumber, PlayerScore>();
        
        foreach (KeyValuePair<PlayerNumber, PlayerScore> item in PlayerScores)
        {
            int playerCaughtScore = CharacterManager.Instance.MazePlayers[item.Key].TimesCaught * -PlayerCaughtPenaltyValue;
            PlayerScore p = item.Value;
            p.PlayerCaughtScore = playerCaughtScore;
            tempPlayerScores.Add(item.Key, p);
        }
        PlayerScores = tempPlayerScores;
    }

    public void ResetMazeLevelScore()
    {
        PlayerScores.Clear();
    }
}
