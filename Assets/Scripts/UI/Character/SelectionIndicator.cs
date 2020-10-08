using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionIndicator : MonoBehaviour
{
    public Transform SelectedObject;

    public void Update()
    {
        transform.position = SelectedObject.position;
    }
}
