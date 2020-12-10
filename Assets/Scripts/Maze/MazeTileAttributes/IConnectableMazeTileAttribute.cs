using UnityEngine;

public interface ITileConnectable
{
    int ConnectionScore { get; set; }

    void WithConnectionScore(int score);
    string GetSubtypeAsString();
}
