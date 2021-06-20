using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorTileAreaEntry : MonoBehaviour
{
    public InputField TileAreaEntryInputField;

    public void Awake()
    {
        Guard.CheckIsNull(TileAreaEntryInputField, "TileAreaEntryInputField");
    }

    public void Delete()
    {
        TileAreaActionHandler.Instance.TileAreaEntries.Remove(this);

        GameObject.Destroy(gameObject);
        GameObject.Destroy(this);
    }
}
