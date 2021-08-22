using UnityEngine;

public static class ScreenCalculator
{
    public static Vector3 GetScreenMiddle()
    {
        return new Vector3(Screen.width / 2, Screen.height / 2);
    }
    public static Vector3 GetSplitScreenScreen1Middle()
    {
        return new Vector3(Screen.width / 4, Screen.height / 2);
    }

    public static Vector3 GetSplitScreenScreen2Middle()
    {
        return new Vector3(Screen.width - Screen.width / 4, Screen.height / 2);
    }
}