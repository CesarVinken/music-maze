using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MapInteractionButton : MonoBehaviour, IMapInteractionButton
{
    [SerializeField] private Text _mapTextLabel;
    private Vector2 _buttonWorldBasePosition;
    [SerializeField] private OverworldPlayerCharacter _triggerPlayer;

    private void Awake()
    {
        Guard.CheckIsNull(_mapTextLabel, "MapTextLabel", gameObject);
    }

    public void Update()
    {
        if (gameObject.activeSelf)
        {
            transform.position = new Vector2(_buttonWorldBasePosition.x + 1, _buttonWorldBasePosition.y);
        }
    }

    private void SetMapInteractionButtonLabel(string mapText)
    {
        _mapTextLabel.text = mapText;
    }

    public void ShowMapInteractionButton(OverworldPlayerCharacter player, Vector2 pos, string mapText)
    {
        _buttonWorldBasePosition = pos;
        SetMapInteractionButtonLabel(mapText);
        transform.position = new Vector2(_buttonWorldBasePosition.x + 1, _buttonWorldBasePosition.y);
        gameObject.SetActive(true);
    }

    public void HideMapInteractionButton()
    {
        SetMapInteractionButtonLabel("");
        gameObject.SetActive(false);
    }

    public void ExecuteMapInteraction()
    {
        string mazeName = _triggerPlayer.OccupiedMazeLevelEntry.MazeLevelName;
        _triggerPlayer.PerformMazeLevelEntryAction(mazeName);
    }

    public bool IsActive()
    {
        return gameObject.activeSelf;
    }
}
