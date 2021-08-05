using UnityEngine;
using UnityEngine.UI;

public class EditorIssueUI : MonoBehaviour
{
    [SerializeField] private Text _issueDescription;
    public EditorIssueType EditorIssueType;
    public GridLocation EditorIssueGridLocation;

    private Vector3 _worldPosition;

    public void Update()
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(_worldPosition);
        transform.position = screenPos;
    }

    public void SetGridLocation(GridLocation gridLocation)
    {
        EditorIssueGridLocation = gridLocation;
    }

    public void SetWorldPosition(Vector3 worldPosition)
    {
        _worldPosition = worldPosition;
    }

    public void SetDescription(string description)
    {
        _issueDescription.text = description;
    }

    public void SetEditorIssueType(EditorIssueType editorIssueType)
    {
        EditorIssueType = editorIssueType;
    }

    public void Delete()
    {
        Destroy(gameObject);
    }
}
