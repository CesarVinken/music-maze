using UnityEngine;

public class SceneObjectManager : MonoBehaviour
{
    public static SceneObjectManager Instance;

    public Transform CharactersGO;

    void Awake()
    {
        Instance = this;

        if (CharactersGO == null)
            Logger.Error(Logger.Initialisation, "Could not find CharactersGO component on SceneObjectManager");
    }

}
