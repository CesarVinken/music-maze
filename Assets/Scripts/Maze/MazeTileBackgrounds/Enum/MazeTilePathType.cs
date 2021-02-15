public interface IOverworld
{
}

public interface IMazeLevel
{
}

public interface IPathType
{
    string Name { get; }
}

public class OverworldDefaultPathType : IPathType, IOverworld
{ 
    public string Name { get =>"DefaultOverworldPath"; }
}

public class MazeLevelDefaultPathType : IPathType, IMazeLevel
{
    public string Name { get => "DefaultMazePath"; }

}

public interface IBaseBackgroundType
{
    string Name { get; }
}

public class OverworldDefaultBaseBackgroundType : IBaseBackgroundType, IOverworld
{
    public string Name { get => "DefaultOverworldBackground"; }
}

public class MazeLevelDefaultBaseBackgroundType : IBaseBackgroundType, IMazeLevel
{
    public string Name { get => "DefaultMazeBackground"; }
}