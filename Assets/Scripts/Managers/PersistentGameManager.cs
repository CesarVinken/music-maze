using Character;
using System.Collections.Generic;

public static class PersistentGameManager
{
    public const string VersionNumber = "0.0.5.3";

    public static Platform CurrentPlatform;
    public static SceneType CurrentSceneType;
    public static SceneLoadOrigin SceneLoadOrigin = SceneLoadOrigin.Gameplay;
    private static string _currentMazeLevelName = "";
    private static string _overworldName = "";
    private static string _originMazeLevelName = "";

    public static Dictionary<PlayerNumber, int> PlayerOveralScores = new Dictionary<PlayerNumber, int>();

    public static string CurrentSceneName { get => _currentMazeLevelName; private set => _currentMazeLevelName = value; }
    public static string OverworldName { get => _overworldName; private set => _overworldName = value; }
    public static string LastMazeLevelName { get => _originMazeLevelName; private set => _originMazeLevelName = value; }

    public static Dictionary<PlayerNumber, string> PlayerCharacterNames = new Dictionary<PlayerNumber, string>();

    public static void SetCurrentSceneName(string currentMazeLevelName)
    {
        _currentMazeLevelName = currentMazeLevelName;
    }

    public static void SetOverworldName(string overworldName)
    {
        _overworldName = overworldName;
    }

    public static void SetLastMazeLevelName(string originMazeLevelName)
    {
        _originMazeLevelName = originMazeLevelName;
    }

    public static void UpdatePlayerOveralScoresWithMazeScore(PlayerNumber playerNumber, int mazeScore)
    {
        if (!PlayerOveralScores.ContainsKey(playerNumber))
        {
            PlayerOveralScores.Add(playerNumber, mazeScore);
            Logger.Log($"PlayerOveralScore: {PlayerOveralScores[playerNumber]}");
        }
        else
        {
            int currentFullScore = PlayerOveralScores[playerNumber];
            PlayerOveralScores[playerNumber] = currentFullScore + mazeScore;
        }
    }
}