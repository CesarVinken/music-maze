using System.Collections.Generic;

public interface IConfigureCommand
{
    void Configure(List<string> arguments);
}