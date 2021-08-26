public interface IEditorLevel : IGameScene<Tile>
{
    bool UnsavedChanges { get; set; }
}
