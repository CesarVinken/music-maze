using Character.CharacterType;
using System;
using UnityEngine;

namespace Character
{
    public class PlayerColour
    {
        public static Color GetColor(ICharacter character)
        {
            switch (character.GetType())
            {
                case Type t when t == typeof(Emmon):
                    return new Color(175f / 255f, 58f / 255f, 66f / 255f);
                case Type t when t == typeof(Fae):
                    return new Color(87f / 255f, 107f / 255f, 153f / 255f);
                default:
                    return new Color(0, 0, 0);
            }
        }
    }
}