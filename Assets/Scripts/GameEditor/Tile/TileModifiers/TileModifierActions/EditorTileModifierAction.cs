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
            case TileModifierActionType.GenerateFullTileTransformationMap:
                MazeTileFullTransformationMapper.GenerateFullTileTransformationMap();
                break;
            case TileModifierActionType.GenerateTileTransformationMapForTile:
                MazeTileTransformationMapperForTile.GenerateTileTransformationMapForTile();
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

