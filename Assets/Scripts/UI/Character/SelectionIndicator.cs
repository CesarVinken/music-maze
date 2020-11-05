using UnityEngine;

public class SelectionIndicator : MonoBehaviour
{
    public Transform SelectedObject;
    private PlayerCharacter _playerCharacter;

    public void Update()
    {
        transform.position = SelectedObject.position;
    }

    public void Setup(Transform selectedObject, PlayerCharacter player) 
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
