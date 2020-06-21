using System.Collections.Generic;
using UnityEngine;

public class CharacterBlueprint
{
    public CharacterType CharacterType;
    public bool IsPlayable 
    { 
        get
        {
            if (CharacterType == CharacterType.Bard) return true;
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
    Bard,
    Dragon
}
