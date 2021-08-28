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
                    return new Color(175, 58, 66);
                case Type t when t == typeof(Fae):
                    return new Color(87, 107, 153);
                default:
                    return new Color(0, 0, 0);
            }
        }
    }
}