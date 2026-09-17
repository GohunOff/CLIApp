using MC.Code.CLI.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC.Code.CLI.Help
{
    public sealed class CommandDiscoveryResult
    {
        public IReadOnlyList<CliCommandDefinition> Commands { get; }

        public DefaultCommand DefaultCommand { get; }

        public CommandDiscoveryResult(
            IReadOnlyList<CliCommandDefinition> commands,
            DefaultCommand defaultCommand)
        {
            Commands = commands
                ?? throw new ArgumentNullException(nameof(commands));

            DefaultCommand = defaultCommand;
        }
    }
}
