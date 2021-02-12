using UnityEngine;

public class EditorOverworldTileModificationPanel : MonoBehaviour, IEditorModificationPanel
{
    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
