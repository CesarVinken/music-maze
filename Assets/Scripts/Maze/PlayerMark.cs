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
        { 17, 2 },
        { 18, 3 },
        { 19, 4 },
        { 20, 5 },
        { 21, 6 },
        { 22, 7 },
        { 23, 8 },
        { 24, 10 },
        { 25, 9 },
        { 26, 11 },
        { 27, 7 },
        { 28, 10 },
        { 29, 7 },
        { 30, 10 },
        { 31, 12 },
        { 32, 13 },
        { 33, 14 },
        { 34, 15 },
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