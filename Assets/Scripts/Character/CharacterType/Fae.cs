using UnityEngine;

namespace CharacterType
{
    public class Fae : ICharacter
    {
        public RuntimeAnimatorController GetAnimationController()
        {
            return GameManager.Instance.CharacterManager.Bard2Controller;
        }

        public string GetPrefabPath()
        {
            if(GameRules.GamePlayerType == GamePlayerType.NetworkMultiplayer)
            {
                if (PersistentGameManager.CurrentSceneType == SceneType.Maze)
                {
                    return "Prefabs/Character/MazeNetworkFae";
                }
                else
                {
                    return "Prefabs/Character/OverworldNetworkFae";
                }
            }

            // GAME TYPE IS NOT NETWORK BUT SINGLEPLAYER OR SPLIT SCREEN
            if (PersistentGameManager.CurrentSceneType == SceneType.Maze)
            {
                return "Prefabs/Character/MazePlayerCharacter";
            }
            else
            {
                return "Prefabs/Character/OverworldPlayerCharacter";
            }
        }
    }
}