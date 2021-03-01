using UnityEngine;

public interface ICharacter
{
    RuntimeAnimatorController GetAnimationController();
    string GetPrefabPath();
}