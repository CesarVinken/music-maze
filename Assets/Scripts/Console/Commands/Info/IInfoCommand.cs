using System.Collections.Generic;

public interface IInfoCommand
{
    string GetInfo(List<string> arguments);
}