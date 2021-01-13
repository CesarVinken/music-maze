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
