using MC.Code.CLI.Command;
using MC.Code.CLI.Help;

namespace MC.Code.CLI.Base
{
    public sealed class CliCommandContext
    {
        public CliCommandDefinition Command { get; }

        public CliCommandContext(
            CliCommandDefinition command)
        {
            Command = command;
        }
    }
}