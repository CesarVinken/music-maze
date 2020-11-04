using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    public int Player1Score = 0;
    public int Player2Score = 0;

    public const int MarkedTileValue = 10;

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
        Logger.Log("Finished counting score.");
    }

    public void CountTileMarkerScore()
    {
        List<Tile> markedTiles = MazeLevelManager.Instance.Level.Tiles.Where(t => t.PlayerMark != null).ToList();
        for (int i = 0; i < markedTiles.Count; i++)
        {
            PlayerMark playerMark = markedTiles[i].PlayerMark;
            if(playerMark.Owner == PlayerMarkOwner.Player1)
            {
                Player1Score += MarkedTileValue;
            }
            else if (playerMark.Owner == PlayerMarkOwner.Player2)
            {
                Player2Score += MarkedTileValue;
            }
        }

        if(CharacterManager.Instance.MazePlayers.Count == 2)
            Logger.Log(Logger.Level, $"Finished counting tile marker scores. Player 1 has {Player1Score} points and player 2 has {Player2Score} points.");
        else
            Logger.Log(Logger.Level, $"Finished counting tile marker scores. Player 1 has {Player1Score} points.");
    }
}
