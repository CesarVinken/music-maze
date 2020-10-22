using System.Collections.Generic;

public abstract class CommandProcedure
{
    public abstract void Run(List<string> arguments);
    public abstract void Help();
}
