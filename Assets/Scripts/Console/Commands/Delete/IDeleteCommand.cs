using System.Collections.Generic;

public interface IDeleteCommand
{
    string Delete(List<string> arguments);
}