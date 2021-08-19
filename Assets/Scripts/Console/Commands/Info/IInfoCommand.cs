using System.Collections.Generic;

namespace Console
{
    public interface IInfoCommand
    {
        string GetInfo(List<string> arguments);
    }
}