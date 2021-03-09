using UnityEngine;

public interface IMapInteractionButton
{
    void ShowMapInteractionButton(OverworldPlayerCharacter triggerPlayer, Vector2 pos, string mapText);
    void ExecuteMapInteraction();
    void HideMapInteractionButton();
}
