using Character;

public static class CharacterHelper
{
    public static PlayerCharacter GetUnbiasedPlayerCharacter(PlayerNumber playerNumber)
    {
        if (PersistentGameManager.CurrentSceneType == SceneType.Maze)
        {
            return GameManager.Instance.CharacterManager.GetPlayers<MazePlayerCharacter>()[playerNumber];
        }
        else
        {
            return GameManager.Instance.CharacterManager.GetPlayers<OverworldPlayerCharacter>()[playerNumber];
        }
    }
}
