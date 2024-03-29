﻿using Character;
using UnityEngine;

namespace UI
{
    public interface IMapInteractionButton
    {
        void Initialise(PlayerCharacter triggerPlayer, Vector2 pos, MapInteractionAction mapInteractionAction, string mapText, Sprite sprite = null);
        void ExecuteMapInteraction(MapInteractionAction actionToExecute);
        void DestroyMapInteractionButtonGO();
    }
}