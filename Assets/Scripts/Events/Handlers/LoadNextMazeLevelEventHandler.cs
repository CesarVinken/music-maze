namespace Gameplay
{
    public class LoadNextMazeLevelEventHandler : IGameplayEventHandler
    {
        private IGameplayManager _gameplayManager;

        public LoadNextMazeLevelEventHandler(IGameplayManager gameplayManager)
        {
            _gameplayManager = gameplayManager;
        }

        public void Handle(object[] data)
        {
            string pickedLevel = (string)data[0];

            PersistentGameManager.SetCurrentSceneName(pickedLevel);

            _gameplayManager.StartNextSceneRoutine("Maze");
        }
    }
}
