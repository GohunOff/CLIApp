using MC.Code.CLI.Application;
using MC.Code.CLI.ARGS;
using MC.Code.CLI.Base;
using MC.Code.CLI.Command;
using MC.Code.CLI.Help;
using MC.Code.CLI.Presentation;
using System;
using System.Linq;

namespace MC.Code.CLI
{
    public static class CLIApp
    {
        public enum NoArgsBehavior
        {
            Run,
            ShowHelp
        }

        private static ApplicationInfo _info;
        private static ApplicationHelp _help;

        private static IApplicationInfoOutput _output;
        private static ParsedCommand _parsedCommand;

        public static ParsedCommand ParsedCommand => _parsedCommand;

        private static NoArgsBehavior _noArgsBehavior;
        public static void Init(
            string description,
            IApplicationInfoOutput output,
            NoArgsBehavior noArgsBehavior,
            params string[] inArgs)
        {
            _output = output
                ?? throw new ArgumentNullException(nameof(output));

            _noArgsBehavior = noArgsBehavior;
            _info = new ApplicationInfo(description);
            _help = new ApplicationHelp();

            _parsedCommand = CommandLineParser.Parse(
                inArgs ?? Array.Empty<string>());
        }

        private static bool ShowHelpCondition()
        {
            if (string.IsNullOrEmpty(_parsedCommand.Name))
                return _noArgsBehavior == NoArgsBehavior.ShowHelp;

            return IsHelpCommand(_parsedCommand.Name);
        }

        private static bool IsHelpCommand(string commandName)
        {
            return CliCommand
                .LoadCliCommands()
                .Any(x => x.IsHelp && 
                x.Command.Equals(
                        commandName,
                        StringComparison.OrdinalIgnoreCase));
        }

        public static bool TryShowHelp()
        {
            if (!ShowHelpCondition())
                return false;

            _output.ShowInfo(_info);
            _output.ShowHelp(_help);

            return true;
        }

        public static void Run()
        {
            var runner = new ApplicationCommandRunner();
            if (!runner.Execute(_parsedCommand) &&
                    _noArgsBehavior == NoArgsBehavior.ShowHelp)
            {
                _output.ShowInfo(_info);
                _output.ShowHelp(_help);
            }
        }
    }
}
