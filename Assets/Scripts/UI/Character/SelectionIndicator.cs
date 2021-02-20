using UnityEngine;

public class SelectionIndicator : MonoBehaviour
{
    public Transform SelectedObject;
    private MazePlayerCharacter _playerCharacter;

    public void Update()
    {
        transform.position = SelectedObject.position;
    }

    public void Setup(Transform selectedObject, MazePlayerCharacter player) 
    {
        SelectedObject = selectedObject;
        _playerCharacter = player;
        _playerCharacter.PlayerExitsEvent += OnPlayerExit;
    }

    public void OnPlayerExit()
    {
        Destroy(gameObject);
    }
}
