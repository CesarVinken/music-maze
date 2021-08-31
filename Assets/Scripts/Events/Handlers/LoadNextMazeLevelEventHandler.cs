using DataSerialisation;

namespace Gameplay
{
    public class LoadNextMazeLevelEventHandler : IGameplayEventHandler
    {
        private MazeLevelGameplayManager _mazeLevelGameplayManager;

        public LoadNextMazeLevelEventHandler(MazeLevelGameplayManager mazeLevelGameplayManager)
        {
            _mazeLevelGameplayManager = mazeLevelGameplayManager;
        }

        public void Handle(object[] data)
        {
            string pickedLevel = (string)data[0];

            MazeLevelData mazeLevelData = MazeLevelLoader.LoadMazeLevelData(pickedLevel);

            if (mazeLevelData == null)
            {
                Logger.Error($"Could not load maze level data for the randomly picked maze level {pickedLevel}");
            }

            PersistentGameManager.SetCurrentSceneName(pickedLevel);

            _mazeLevelGameplayManager.StartOverworldCoroutine("Overworld");
        }
    }
}