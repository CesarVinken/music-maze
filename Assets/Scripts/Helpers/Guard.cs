using System;
using UnityEngine;
using UnityEngine.UI;

public class Guard
{
    public static void CheckIsNull(GameObject gameObject)
    {
        if (gameObject == null)
            Logger.Error($"Cannot find {gameObject.name}");
    }

    public static void CheckIsNull(GameObject gameObject, string name)
    {
        if (gameObject == null)
            Logger.Error($"Cannot find {name}");
    }

    public static void CheckIsNull<T>(T component, string name)
    {
        Type typeParameterType = typeof(T);
        if (component == null)
            Logger.Error($"Cannot find {typeParameterType} {name}");
    }

    public static void CheckIsNull<T>(T component, string name, GameObject parent)
    {
        Type typeParameterType = typeof(T);
        if (component == null)
            Logger.Error($"Cannot find {typeParameterType} {name} on {parent.name}");
    }

    //public static void CheckIsNull(SpriteRenderer spriteRenderer, string name)
    //{
    //    if (spriteRenderer == null)
    //        Logger.Error("Cannot find spriteRenderer {0}", name);
    //}

    //public static void CheckIsNull(Sprite sprite, string name)
    //{
    //    if (sprite == null)
    //        Logger.Error("Cannot find sprite {0}", name);
    //}

    //public static void CheckIsNull(InputField inputField, string name)
    //{
    //    if (inputField == null)
    //        Logger.Error("Cannot find inputField {0}", name);
    //}

    public static void CheckIsEmptyString(string name, string content)
    {
        if(content == "")
            Logger.Error("{0} cannot be an empty string", name);
    }

    public static void CheckLength(Sprite[] sprite, string name)
    {
        if(sprite.Length == 0)
            Logger.Error($"the array {name} should not have a length of 0");
    }
}
