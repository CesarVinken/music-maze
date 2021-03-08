public class SerialisableMazeLevelEntryAttribute : SerialisableTileAttribute
{
    public string MazeLevelName;
    public SerialisableMazeLevelEntryAttribute(string mazeLevelName)
    {
        TileAttributeId = MazeLevelEntryCode;
        MazeLevelName = mazeLevelName;
    }
}
