namespace Gameplay
{
    public class LoadOverworldEventHandler : IGameplayEventHandler
    {
        private MazeLevelGameplayManager _mazeLevelGameplayManager;

        public LoadOverworldEventHandler(MazeLevelGameplayManager mazeLevelGameplayManager)
        {
            _mazeLevelGameplayManager = mazeLevelGameplayManager;
        }

        public void Handle(object[] data)
        {
            string overworldName = (string)data[0];

            PersistentGameManager.SetLastMazeLevelName(PersistentGameManager.CurrentSceneName);
            PersistentGameManager.SetCurrentSceneName(PersistentGameManager.OverworldName);

            _mazeLevelGameplayManager.StartNextSceneRoutine("Overworld");
        }
    }
}