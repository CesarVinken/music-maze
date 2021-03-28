using Photon.Pun;

public class GameLaunchAction
{
    public void Launch()
    {
        if (GameRules.GameMode == GameMode.Campaign)
        {
            PersistentGameManager.SetOverworldName("overworld");
            PersistentGameManager.SetCurrentSceneName(PersistentGameManager.OverworldName);

            PhotonNetwork.LoadLevel("Overworld");
        }
        else
        {
            PersistentGameManager.SetLastMazeLevelName("default");
            PersistentGameManager.SetCurrentSceneName("default");

            PhotonNetwork.LoadLevel("Maze");
        }
    }
}