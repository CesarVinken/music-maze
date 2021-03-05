using UnityEngine;

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