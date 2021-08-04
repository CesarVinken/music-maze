using System.Collections.Generic;
using System.Linq;

public struct PlayerMazeScore
{
    public int TileMarkScore;
    public int PlayerCaughtScore;
    public int EnemiesStartledScore;
    public int FinishFirstBonusScore;

    public int MazeScore;

    public PlayerMazeScore(int tileMarkScore = 0, int playerCaughtScore = 0, int enemiesStartledScore = 0, int finishFirstBonusScore = 0)
    {
        TileMarkScore = tileMarkScore;
        PlayerCaughtScore = playerCaughtScore;
        EnemiesStartledScore = enemiesStartledScore;
        FinishFirstBonusScore = finishFirstBonusScore;

        MazeScore = 0;
    }

    public int CountMazeTotal()
    {
        MazeScore = TileMarkScore + PlayerCaughtScore + EnemiesStartledScore + FinishFirstBonusScore;
        return MazeScore;
    }
}

public class MazeScoreCalculator
{
    public const int MarkedTileValue = 10;
    public const int EnemyMadeListenToMusicValue = 50;
    public const int PlayerCaughtPenaltyValue = 20;

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
        CountEnemyEncountersScores();
        CountFirstFinishedBonus();

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

        List<InGameMazeTile> markedTiles = new List<InGameMazeTile>();
        for (int i = 0; i < MazeLevelGameplayManager.Instance.Level.Tiles.Count; i++)
        {
            InGameMazeTile tile = MazeLevelGameplayManager.Instance.Level.Tiles[i] as InGameMazeTile;
            if(tile.PlayerMark != null)
            {
                markedTiles.Add(tile);
            }
        }

        int playerMarkScorePlayer1 = 0;
        int playerMarkScorePlayer2 = 0;

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

    private void CountEnemyEncountersScores()
    {
        MazeCharacterManager characterManager = GameManager.Instance.CharacterManager as MazeCharacterManager;

        if (characterManager == null) return;

        Dictionary<PlayerNumber, PlayerMazeScore> tempPlayerScores = new Dictionary<PlayerNumber, PlayerMazeScore>();

        foreach (KeyValuePair<PlayerNumber, PlayerMazeScore> item in PlayerMazeScores)
        {
            Dictionary<PlayerNumber, MazePlayerCharacter> players = characterManager.GetPlayers<MazePlayerCharacter>();
            int playerCaughtScore = players[item.Key].TimesCaughtByEnemy * -PlayerCaughtPenaltyValue;
            int EnemyStartledScore = players[item.Key].TimesMadeEnemyListenToMusicInstrument * EnemyMadeListenToMusicValue; // there can later be multiple ways to startle an enemy
            PlayerMazeScore p = item.Value;
            p.PlayerCaughtScore = playerCaughtScore;
            p.EnemiesStartledScore = EnemyStartledScore;
            tempPlayerScores.Add(item.Key, p);
        }
        PlayerMazeScores = tempPlayerScores;
    }

    private void CountFirstFinishedBonus()
    {
        Dictionary<PlayerNumber, MazePlayerCharacter> players = GameManager.Instance.CharacterManager.GetPlayers<MazePlayerCharacter>();
        Dictionary<PlayerNumber, PlayerMazeScore> tempPlayerScores = new Dictionary<PlayerNumber, PlayerMazeScore>();

        foreach (KeyValuePair<PlayerNumber, PlayerMazeScore> item in PlayerMazeScores)
        {
            int finishFirstBonusScore = players[item.Key].FinishedFirstBonus ? 50 : 0;
            PlayerMazeScore p = item.Value;
            p.FinishFirstBonusScore = finishFirstBonusScore;
            tempPlayerScores.Add(item.Key, p);
        }

        PlayerMazeScores = tempPlayerScores;
    }

    public void ResetMazeLevelScore()
    {
        PlayerMazeScores.Clear();
    }
}
