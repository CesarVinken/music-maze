using Character;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class MainScreenCameraCanvas : MonoBehaviour
    {
        public static MainScreenCameraCanvas Instance;

        public GameObject MapInteractionButtonPrefab;
        public List<MapInteractionButton> MapInteractionButtons = new List<MapInteractionButton>();

        private void Awake()
        {
            Guard.CheckIsNull(MapInteractionButtonPrefab, "MapInteractionButtonPrefab", gameObject);

            Instance = this;
        }

        // Make generic so different types of button interactions can be used, not just Maze Entry
        public void CreateMapInteractionButton(PlayerCharacter player, Vector2 pos, MapInteractionAction mapInteractionAction, string mapText, Sprite sprite)
        {
            Logger.Log($"Create map interaction for player {player.PlayerNumber}");

            GameObject mapInteractionButtonGO = Instantiate(MapInteractionButtonPrefab, transform);
            MapInteractionButton mapInteractionButton = mapInteractionButtonGO.GetComponent<MapInteractionButton>();

            mapInteractionButton.ShowMapInteractionButton(player, pos, mapInteractionAction, mapText, sprite);

            MapInteractionButtons.Add(mapInteractionButton);
            player.MapInteractionButtonsForPlayer.Add(mapInteractionButton);
        }

        public void DestroyMapMapInteractionButton(OverworldPlayerCharacter player)
        {
            Logger.Log($"Destroy map interaction label for player {player.PlayerNumber}");
            for (int i = 0; i < MapInteractionButtons.Count; i++)
            {
                if (MapInteractionButtons[i].TriggerPlayer.PlayerNumber == player.PlayerNumber)
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
}