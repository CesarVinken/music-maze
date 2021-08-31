using UnityEngine;

public interface IGameplayManager
{
    GameObject GetTileBackgroundPrefab<T>() where T : ITileBackground;
    GameObject GetTileAttributePrefab<T>() where T : ITileAttribute;

    void StartNextSceneRoutine(string levelName);
}