using Character;
using UnityEngine;

namespace UI
{
    public interface IMapInteractionButton
    {
        void ShowMapInteractionButton(OverworldPlayerCharacter triggerPlayer, Vector2 pos, string mapText);
        void ExecuteMapInteraction();
        void DestroyMapInteractionButton();
    }
}