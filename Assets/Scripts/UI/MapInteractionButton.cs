using UnityEngine;
using UnityEngine.UI;


public class MapInteractionButton : MonoBehaviour, IMapInteractionButton
{
    [SerializeField] private Text _mapTextLabel;
    private Vector2 _buttonWorldBasePosition;
    [SerializeField] public OverworldPlayerCharacter TriggerPlayer;
    private Camera _cameraToUse = null;


    private void Awake()
    {
        Guard.CheckIsNull(_mapTextLabel, "MapTextLabel", gameObject);
    }

    public void Update()
    {
        if (gameObject.activeSelf)
        {
            Vector2 positionAdjustedForScreenPoint = _cameraToUse.WorldToScreenPoint(new Vector2(_buttonWorldBasePosition.x + 0.5f, _buttonWorldBasePosition.y + 1));
            transform.position = new Vector2(positionAdjustedForScreenPoint.x, positionAdjustedForScreenPoint.y);
        }
    }

    private void SetMapInteractionButtonLabel(string mapText)
    {
        _mapTextLabel.text = mapText;
    }

    public void ShowMapInteractionButton(OverworldPlayerCharacter player, Vector2 pos, string mapText)
    {
        _buttonWorldBasePosition = pos;
        TriggerPlayer = player;

        if (GameRules.GamePlayerType == GamePlayerType.SplitScreenMultiplayer &&
            player.PlayerNumber == PlayerNumber.Player2)
        {
            _cameraToUse = CameraManager.Instance.CameraControllers[1].GetCamera();
        }
        else
        {
            _cameraToUse = CameraManager.Instance.CameraControllers[0].GetCamera();
        }

        Vector2 positionAdjustedForScreenPoint = _cameraToUse.WorldToScreenPoint(new Vector2(_buttonWorldBasePosition.x + 0.5f, _buttonWorldBasePosition.y + 1));

        SetMapInteractionButtonLabel(mapText);
        transform.position = new Vector2(positionAdjustedForScreenPoint.x, positionAdjustedForScreenPoint.y);
        gameObject.SetActive(true);
    }

    public void DestroyMapInteractionButton()
    {
        Destroy(gameObject);
    }

    public void ExecuteMapInteraction()
    {
        string mazeName = TriggerPlayer.OccupiedMazeLevelEntry.MazeLevelName;
        TriggerPlayer.PerformMazeLevelEntryAction(mazeName);
    }

    public bool IsActive()
    {
        return gameObject.activeSelf;
    }
}
