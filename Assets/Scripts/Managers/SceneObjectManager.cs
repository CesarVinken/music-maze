using UnityEngine;

public class SceneObjectManager : MonoBehaviour
{
    public static SceneObjectManager Instance;

    public Transform CharactersGO;

    void Awake()
    {
        Instance = this;

        Guard.CheckIsNull(CharactersGO, "CharactersGO", gameObject);
    }

}
