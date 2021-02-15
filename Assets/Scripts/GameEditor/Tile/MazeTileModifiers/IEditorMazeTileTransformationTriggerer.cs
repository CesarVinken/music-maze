public interface IEditorTileTransformationTriggerer : IEditorTileModifierType
{
    string Name { get; }

    void HandleTransformationTriggerPlacement(EditorMazeTile tile);
}
