public class SerialisableMazeLevelEntryAttribute : ISerialisableTileAttribute
{
    public string MazeLevelName;

    public SerialisableMazeLevelEntryAttribute(string mazeLevelName)
    {
        MazeLevelName = mazeLevelName;
    }
}
