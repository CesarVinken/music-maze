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
            if (GameManager.CurrentSceneType == SceneType.Maze)
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