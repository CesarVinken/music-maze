using System.Collections.Generic;

public class PlayerMark
{
    public int ConnectionScore = -1;
    public PlayerMarkOwner Owner { get; private set; }

    private static Dictionary<int, int> _pathConnectionScoreMarkerScoreTable = new Dictionary<int, int>
    {
        { 1, 1 },
        { 2, 2 },
        { 3, 3 },
        { 4, 4 },
        { 5, 5 },
        { 6, 6 },
        { 7, 7 },
        { 8, 8 },
        { 9, 9 },
        { 10, 10 },
        { 11, 11 },
        { 12, 12 },
        { 13, 13 },
        { 14, 14 },
        { 15, 15 },
        { 16, 16 },
        { 17, 17 },
        { 18, 18 },
        { 19, 19 },
        { 20, 20 },
        { 21, 21 },
        { 22, 22 },
        { 23, 23 },
        { 24, 24 },
        { 25, 25 },
        { 26, 26 },
        { 27, 27 },
        { 28, 28 },
        { 29, 29 },
        { 30, 30 },
        { 31, 31 },
        { 32, 32 },
        { 33, 33 },
        { 34, 34 },
    };

    public PlayerMark(int pathConnectionScore)
    {
        Owner = PlayerMarkOwner.None;

        ConnectionScore = ConvertPathScoreToMarkerScore(pathConnectionScore);
    }

    public void SetOwner(PlayerMarkOwner owner)
    {
        Owner = owner;
    }

    public static int ConvertPathScoreToMarkerScore(int pathConnectionScore)
    {
        if (_pathConnectionScoreMarkerScoreTable.TryGetValue(pathConnectionScore, out int markerScore))
        {
            return markerScore;
        }
        Logger.Error($"Unknown markerScore value for pathConnectionScore {pathConnectionScore}");
        return -1;
    }
}