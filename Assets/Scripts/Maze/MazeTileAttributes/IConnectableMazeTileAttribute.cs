using UnityEngine;

public interface ITileConnectable
{
    int ConnectionScore { get; set; }
    int SpriteNumber { get; set; }

    void WithConnectionScoreInfo(TileConnectionScoreInfo score);

    string GetSubtypeAsString();
}
