using System.Collections.Generic;

namespace Console
{
    public interface IDeleteCommand
    {
        string Delete(List<string> arguments);
    }
}