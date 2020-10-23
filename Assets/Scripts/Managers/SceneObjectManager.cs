using System.Collections.Generic;
using UnityEngine;

public class SceneObjectManager : MonoBehaviour
{
    public static SceneObjectManager Instance;

    public Transform CharactersGO;

    public List<GameObject> SceneObjects = new List<GameObject>();

    void Awake()
    {
        Instance = this;

        Guard.CheckIsNull(CharactersGO, "CharactersGO", gameObject);
    }

    public void UnloadSceneObjects()
    {
        for (int i = SceneObjects.Count - 1; i >= 0; i--)
        {
            Destroy(SceneObjects[i]);
        }
        SceneObjects.Clear();
    }
}
