using System.Collections.Generic;

namespace MC.Code.CLI.Help
{
    public sealed class ApplicationHelp
    {
        private readonly List<CliCommandDefinition> _items = new List<CliCommandDefinition>();

        public ApplicationHelpFormat Format { get; }

        public IReadOnlyList<CliCommandDefinition> Items =>
            _items;

        public ApplicationHelp(
            ApplicationHelpFormat format = null)
        {
            Format =
                format ??
                new ApplicationHelpFormat();

            Load();
        }

        private void Load()
        {
            _items.AddRange(Base.CliCommand.LoadCliCommands());
        }
    }
}

