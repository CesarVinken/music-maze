using UnityEngine;

namespace Character
{
    namespace CharacterType
    {
        public class EvilViolin : ICharacter
        {
            public RuntimeAnimatorController GetAnimationController()
            {
                MazeCharacterManager manager = GameManager.Instance.CharacterManager as MazeCharacterManager;
                return manager.EnemyController;
            }

            public string GetPrefabPath()
            {
                return "Prefabs/Character/EnemyCharacter";
            }
        }
    }
}