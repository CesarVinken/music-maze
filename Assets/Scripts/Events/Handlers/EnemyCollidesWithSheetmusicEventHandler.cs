using Character;
using System.Linq;

namespace Gameplay
{
    public class EnemyCollidesWithSheetmusicEventHandler : IGameplayEventHandler
    {
        private MazeLevelGameplayManager _mazeLevelGameplayManager;

        public EnemyCollidesWithSheetmusicEventHandler(MazeLevelGameplayManager mazeLevelGameplayManager)
        {
            _mazeLevelGameplayManager = mazeLevelGameplayManager;
        }

        public void Handle(object[] data)
        {
            GridLocation tileLocation = new GridLocation((int)data[0], (int)data[1]);
            int enemyId = (int)data[2];

            InGameMazeTile tile = _mazeLevelGameplayManager.Level.TilesByLocation[tileLocation] as InGameMazeTile;

            Sheetmusic sheetmusic = (Sheetmusic)tile.GetAttributes().FirstOrDefault(attribute => attribute is Sheetmusic);
            if (sheetmusic == null)
            {
                Logger.Error("Could not find sheetmusic");
            }

            MazeCharacterManager characterManager = GameManager.Instance.CharacterManager as MazeCharacterManager;

            EnemyCharacter enemyCharacter = characterManager.Enemies.FirstOrDefault(enemy => enemy.PhotonView.ViewID == enemyId);
            if (enemyCharacter == null)
            {
                Logger.Error("Could not find enemy character");
            }
            sheetmusic.EnemyCollisionOnTile(enemyCharacter);
        }
    }
}