
using UnityEngine;

public class MainScreenCameraCanvas : MonoBehaviour
{
    public static MainScreenCameraCanvas Instance;

    public GameObject MapTextGO;
    public MapInteractionButton MapInteractionButton;

    private void Awake()
    {
        Guard.CheckIsNull(MapTextGO, "MapTextGO", gameObject);
        Guard.CheckIsNull(MapInteractionButton, "MapText", gameObject);

        Instance = this;
    }

    // Make generic so different types of button interactions can be used, not just Maze Entry
    public void ShowMapInteractionButton(OverworldPlayerCharacter player, Vector2 pos, string mapText)
    {
        MapInteractionButton.ShowMapInteractionButton(player, pos, mapText);
    }

    public void HideMapMapInteractionButton()
    {
        MapInteractionButton.HideMapInteractionButton();
    }
}
