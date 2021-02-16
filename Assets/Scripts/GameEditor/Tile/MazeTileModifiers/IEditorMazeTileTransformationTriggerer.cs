public interface IEditorTileTransformationTriggerer<T> : IEditorTileModifierType where T : Tile
{
    string Name { get; }

    void HandleBeautificationTriggerPlacement(T tile);

}
