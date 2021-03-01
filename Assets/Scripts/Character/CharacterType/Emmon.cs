using UnityEngine;

namespace CharacterType
{
    public class Emmon : ICharacter
    {
        public RuntimeAnimatorController GetAnimationController()
        {
            if (GameManager.CurrentSceneType == SceneType.Maze)
            {
                MazeCharacterManager manager = CharacterManager.Instance as MazeCharacterManager;
                return manager.Bard1Controller;
            }
            else
            {
                OverworldCharacterManager manager = CharacterManager.Instance as OverworldCharacterManager;
                return manager.Bard1Controller;
            }
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