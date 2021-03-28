
public enum GameMode
{
    Campaign,
    RandomMaze
}

public static class GameRules
{
    public static GameMode GameMode;
    public static GamePlayerType GamePlayerType;
    public static bool SplitScreen = true;

    public static void SetGameMode(GameMode gameMode)
    {
        GameMode = gameMode;
    }

    public static void SetGamePlayerType(GamePlayerType gamePlayerType)
    {
        GamePlayerType = gamePlayerType;
    }

    public static void SetSplitScreen(bool splitScreen)
    {
        SplitScreen = splitScreen;
    }
}
