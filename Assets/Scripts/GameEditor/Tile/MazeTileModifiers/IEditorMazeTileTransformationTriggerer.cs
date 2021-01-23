public interface IEditorMazeTileTransformationTriggerer : IEditorMazeTileModifierType
{
    string Name { get; }

    void HandleTransformationTriggerPlacement(EditorTile tile);
}
