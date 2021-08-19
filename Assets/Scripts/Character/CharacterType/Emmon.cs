using UnityEngine;

namespace Character
{
    namespace CharacterType
    {
        public class Emmon : ICharacter
        {
            public RuntimeAnimatorController GetAnimationController()
            {
                return GameManager.Instance.CharacterManager.Bard1Controller;
            }

            public string GetPrefabPath()
            {
                if (GameRules.GamePlayerType == GamePlayerType.NetworkMultiplayer)
                {
                    if (PersistentGameManager.CurrentSceneType == SceneType.Maze)
                    {
                        return "Prefabs/Character/MazeNetworkEmmon";
                    }
                    else
                    {
                        return "Prefabs/Character/OverworldNetworkEmmon";
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
}