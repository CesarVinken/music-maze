using System.Collections.Generic;

namespace Console
{
    public interface IConfigureCommand
    {
        void Configure(List<string> arguments);
    }
}