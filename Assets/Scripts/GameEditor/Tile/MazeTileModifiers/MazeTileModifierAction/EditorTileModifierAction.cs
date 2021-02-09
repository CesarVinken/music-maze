using UnityEngine;

public class EditorTileModifierAction : MonoBehaviour
{
    [SerializeField] TileModifierActionType _tileModifierActionType;
    public void ExecuteAction()
    {
        switch (_tileModifierActionType)
        {
            case TileModifierActionType.GenerateTileTransformationMap:
                MazeTileTransformationMapper.GenerateTileTransformationMap();
                break;
            default:
                Logger.Error("Action type was not yet implemented");
                break;
        }
    }
}
