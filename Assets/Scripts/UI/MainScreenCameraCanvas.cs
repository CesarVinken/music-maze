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
        public Sprite[] FerryBoardingIcons;

        private void Awake()
        {
            Guard.CheckIsNull(MapInteractionButtonPrefab, "MapInteractionButtonPrefab", gameObject);
            Guard.CheckLength(FerryBoardingIcons, "FerryBoardingIcons");

            Instance = this;
        }

        public GameObject CreateMapInteractionButton(PlayerCharacter player, Vector2 pos, MapInteractionAction mapInteractionAction, string mapText, Sprite sprite)
        {
            GameObject mapInteractionButtonGO = Instantiate(MapInteractionButtonPrefab, transform);
            MapInteractionButton mapInteractionButton = mapInteractionButtonGO.GetComponent<MapInteractionButton>();

            mapInteractionButton.Initialise(player, pos, mapInteractionAction, mapText, sprite);

            MapInteractionButtons.Add(mapInteractionButton);
            player.MapInteractionButtonsForPlayer.Add(mapInteractionButton);

            return mapInteractionButtonGO;
        }

        public void DestroyMapInteractionButtonsForPlayer(OverworldPlayerCharacter player)
        {
            for (int i = 0; i < MapInteractionButtons.Count; i++)
            {
                if (MapInteractionButtons[i].TriggerPlayer.PlayerNumber == player.PlayerNumber)
                {
                    MapInteractionButton mapInteractionButton = MapInteractionButtons[i];
                    player.MapInteractionButtonsForPlayer.Remove(mapInteractionButton);
                    MapInteractionButtons.Remove(mapInteractionButton);
                    mapInteractionButton.DestroyMapInteractionButtonGO();
                    break;
                }
            }
        }

        public void DestroyMapInteractionButton(string id)
        {
            for (int i = 0; i < MapInteractionButtons.Count; i++)
            {
                if (MapInteractionButtons[i].Id.Equals(id))
                {
                    MapInteractionButton mapInteractionButton = MapInteractionButtons[i];
                    mapInteractionButton.TriggerPlayer.MapInteractionButtonsForPlayer.Remove(mapInteractionButton);
                    MapInteractionButtons.Remove(MapInteractionButtons[i]);
                    mapInteractionButton.DestroyMapInteractionButtonGO();
                    break;
                }
            }
        }
    }
}