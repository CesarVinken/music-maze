using Character;
using UnityEngine;

namespace UI
{
    public interface IMapInteractionButton
    {
        void ShowMapInteractionButton(PlayerCharacter triggerPlayer, Vector2 pos, MapInteractionAction mapInteractionAction, string mapText, Sprite sprite = null);
        void ExecuteMapInteraction(MapInteractionAction actionToExecute);
        void DestroyMapInteractionButton();
    }
}