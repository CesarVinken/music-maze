public interface IEditorTileTransformationTriggerer : IEditorTileModifierType
{
    string Name { get; }

    void HandleTransformationTriggerPlacement(EditorTile tile);
}
