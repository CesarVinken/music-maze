
using System.Collections.Generic;
using UnityEngine;

public class MainScreenCameraCanvas : MonoBehaviour
{
    public static MainScreenCameraCanvas Instance;

    public GameObject MapTextGO;
    public GameObject MapInteractionButtonPrefab;
    public List<MapInteractionButton> MapInteractionButtons = new List<MapInteractionButton>();

    private void Awake()
    {
        Guard.CheckIsNull(MapTextGO, "MapTextGO", gameObject);
        Guard.CheckIsNull(MapInteractionButtonPrefab, "MapInteractionButtonPrefab", gameObject);

        Instance = this;
    }

    // Make generic so different types of button interactions can be used, not just Maze Entry
    public void CreateMapInteractionButton(OverworldPlayerCharacter player, Vector2 pos, string mapText)
    {
        Logger.Log($"Create map interaction for player {player.PlayerNumber}");

        GameObject mapInteractionButtonGO = GameObject.Instantiate(MapInteractionButtonPrefab, transform);
        MapInteractionButton mapInteractionButton = mapInteractionButtonGO.GetComponent<MapInteractionButton>();

        mapInteractionButton.ShowMapInteractionButton(player, pos, mapText);

        MapInteractionButtons.Add(mapInteractionButton);
        player.MapInteractionButtonsForPlayer.Add(mapInteractionButton);
    }

    public void DestroyMapMapInteractionButton(OverworldPlayerCharacter player)
    {
        Logger.Log($"Destroy map interaction label for player {player.PlayerNumber}");
        for (int i = 0; i < MapInteractionButtons.Count; i++)
        {
            if(MapInteractionButtons[i].TriggerPlayer.PlayerNumber == player.PlayerNumber)
            {
                MapInteractionButton mapInteractionButton = MapInteractionButtons[i];
                player.MapInteractionButtonsForPlayer.Remove(MapInteractionButtons[i]);
                MapInteractionButtons.Remove(MapInteractionButtons[i]);
                mapInteractionButton.DestroyMapInteractionButton();
                break;
            }
        }
    }
}
