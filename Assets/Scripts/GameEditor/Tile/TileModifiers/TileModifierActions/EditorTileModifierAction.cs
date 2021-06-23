using UnityEngine;

public class EditorTileModifierAction : MonoBehaviour
{
    [SerializeField] TileModifierActionType _tileModifierActionType;
    public void ExecuteAction()
    {
        switch (_tileModifierActionType)
        {
            case TileModifierActionType.AssignMazeLevelEntry:
                MazeLevelEntryAssigner.AssignMazeLevelEntry();
                break;
            case TileModifierActionType.GenerateTileTransformationMap:
                MazeTileTransformationMapper.GenerateTileTransformationMap();
                break;
            case TileModifierActionType.CreateNewTileAreaEntry:
                NewTileAreaEntryCreator.CreateNewTileAreaEntry();
                break;
            case TileModifierActionType.AssignTileAreaToEnemySpawnpoint:
                TileAreaToEnemySpawnpointAssigner.AssignTileAreaToEnemySpawnpoint();
                break;
            default:
                Logger.Error("Action type was not yet implemented");
                break;
        }
    }
}

