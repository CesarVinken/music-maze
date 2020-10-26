using UnityEngine;

public class EditorUIContainer : MonoBehaviour
{
    public static EditorUIContainer Instance;

    public GameObject EditorModeStatusTextGO;
    public GameObject EditorUIGO;

    public void Awake()
    {
        Instance = this;

        Guard.CheckIsNull(EditorModeStatusTextGO, "EditorModeStatusTextGO", gameObject);
        Guard.CheckIsNull(EditorUIGO, "EditorUIGO", gameObject);
    }

    public void InitialiseEditor()
    {
        EditorModeStatusTextGO.SetActive(true);
        EditorUIGO.SetActive(true);
    }

    public void CloseEditor()
    {
        EditorModeStatusTextGO.SetActive(false);
        EditorUIGO.SetActive(false);
    }
}
