using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct PlayerMazeScore
{
    public int TileMarkScore;
    public int PlayerCaughtScore;
    public int FinishFirstBonusScore;

    public int MazeScore;

    public PlayerMazeScore(int tileMarkScore = 0, int playerCaughtScore = 0, int finishFirstBonusScore = 0)
    {
        TileMarkScore = tileMarkScore;
        PlayerCaughtScore = playerCaughtScore;
        FinishFirstBonusScore = finishFirstBonusScore;

        MazeScore = 0;
    }

    public int CountMazeTotal()
    {
        MazeScore = TileMarkScore + PlayerCaughtScore + FinishFirstBonusScore;
        return MazeScore;
    }
}

public class MazeScoreCalculator
{
    public const int MarkedTileValue = 10;
    public const int PlayerCaughtPenaltyValue = 10;

    public Dictionary<PlayerNumber, PlayerMazeScore> PlayerMazeScores = new Dictionary<PlayerNumber, PlayerMazeScore>();

    public void CalculateScores()
    {
        Logger.Log(Logger.Score, "Finished counting score.");

        ICharacterManager characterManager = GameManager.Instance.CharacterManager;

        if (characterManager == null) return; 
        
        if (characterManager.GetPlayers<MazePlayerCharacter>().Count == 1)
        {
            PlayerMazeScores.Add(PlayerNumber.Player1, new PlayerMazeScore());
        }
        else
        {
            PlayerMazeScores.Add(PlayerNumber.Player1, new PlayerMazeScore());
            PlayerMazeScores.Add(PlayerNumber.Player2, new PlayerMazeScore());
        }

        CountTileMarkerScores();
        CountTimesCaughtScores();

        Dictionary<PlayerNumber, PlayerMazeScore> tempPlayerScores = new Dictionary<PlayerNumber, PlayerMazeScore>();

        foreach (KeyValuePair<PlayerNumber, PlayerMazeScore> item in PlayerMazeScores)
        {
            int mazeTotal = item.Value.CountMazeTotal();
            PlayerMazeScore playerMazeScore = item.Value;
            playerMazeScore.MazeScore = mazeTotal;
            tempPlayerScores.Add(item.Key, playerMazeScore);

            PersistentGameManager.UpdatePlayerOveralScoresWithMazeScore(item.Key, playerMazeScore.MazeScore);
        }

        PlayerMazeScores = tempPlayerScores;

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
        if (PlayerMazeScores.ContainsKey(PlayerNumber.Player2))
        {
            tempPlayerScores.Add(PlayerNumber.Player2, playerMarkScorePlayer2);
        }

        foreach (KeyValuePair<PlayerNumber, int> item in tempPlayerScores)
        {
            PlayerMazeScore p = PlayerMazeScores[item.Key];
            p.TileMarkScore = item.Value;
            PlayerMazeScores[item.Key] = p;
            Logger.Log(Logger.Score, $"Tile marker scores: {item.Key.ToString()} has {PlayerMazeScores[item.Key].TileMarkScore} points.");
        }
    }

    private void CountTimesCaughtScores()
    {
        Dictionary<PlayerNumber, PlayerMazeScore> tempPlayerScores = new Dictionary<PlayerNumber, PlayerMazeScore>();
        
        foreach (KeyValuePair<PlayerNumber, PlayerMazeScore> item in PlayerMazeScores)
        {
            MazeCharacterManager characterManager = GameManager.Instance.CharacterManager as MazeCharacterManager;

            if (characterManager == null) return;

            Dictionary<PlayerNumber, MazePlayerCharacter> players = characterManager.GetPlayers<MazePlayerCharacter>();
            int playerCaughtScore = players[item.Key].TimesCaught * -PlayerCaughtPenaltyValue;
            PlayerMazeScore p = item.Value;
            p.PlayerCaughtScore = playerCaughtScore;
            tempPlayerScores.Add(item.Key, p);
        }
        PlayerMazeScores = tempPlayerScores;
    }

    public void ResetMazeLevelScore()
    {
        PlayerMazeScores.Clear();
    }
}
