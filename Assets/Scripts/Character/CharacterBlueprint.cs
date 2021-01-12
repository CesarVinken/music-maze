using System.Collections.Generic;
using UnityEngine;

public class CharacterBlueprint
{
    public CharacterType CharacterType;
    public bool IsPlayable 
    { 
        get
        {
            if (CharacterType == CharacterType.Bard1 || CharacterType == CharacterType.Bard2) return true;
            return false;
        }
    }

    public CharacterBlueprint(CharacterType characterType)
    {
        CharacterType = characterType;
    }
}

public enum CharacterType
{
    Bard1,
    Bard2,
    Dragon
}
