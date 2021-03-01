using UnityEngine;

namespace CharacterType
{
    public class EvilViolin : ICharacter
    {
        public RuntimeAnimatorController GetAnimationController()
        {
            MazeCharacterManager manager = CharacterManager.Instance as MazeCharacterManager;
            return manager.EnemyController;
        }

        public string GetPrefabPath()
        {
            return "Prefabs/Character/EnemyCharacter";
        }
    }
}