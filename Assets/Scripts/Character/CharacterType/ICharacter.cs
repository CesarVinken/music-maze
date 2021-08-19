using UnityEngine;
namespace Character
{
    public interface ICharacter
    {
        RuntimeAnimatorController GetAnimationController();
        string GetPrefabPath();
    }
}
