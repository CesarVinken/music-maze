using CharacterType;

public class CharacterBlueprint
{
    public ICharacter CharacterType;
    public bool IsPlayable 
    { 
        get
        {
            if (CharacterType is Emmon || CharacterType is Fae) return true;
            return false;
        }
    }

    public CharacterBlueprint(ICharacter characterType)
    {
        CharacterType = characterType;
    }
}
