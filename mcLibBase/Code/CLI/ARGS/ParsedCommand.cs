using System;
using System.Collections.Generic;

namespace MC.Code.CLI.ARGS
{
    public sealed class ParsedCommand
    {
        public string Name { get; }

        public IReadOnlyList<CommandLineArgument> Items
        {
            get { return _items; }
        }

        private readonly List<CommandLineArgument> _items =
            new List<CommandLineArgument>();

        public int Count
        {
            get { return _items.Count; }
        }

        public CommandLineArgument this[int index]
        {
            get { return _items[index]; }
        }

        private ParsedCommand(string name)
        {
            Name = name;
        }

        public static ParsedCommand Create(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException(
                    "Nazwa komendy nie może być pusta.",
                    nameof(name));

            return new ParsedCommand(name);
        }

        internal void AddArgument(int position,
                                int argumentIndex, string value)
        {
            _items.Add(
                CommandLineArgument.CreateArgument(position,
            argumentIndex, value));
        }

        internal void AddOption(
             int position,
            int optionIndex,
            string name,
            string value)
        {
            _items.Add(
                CommandLineArgument.CreateOption(
                    position,
                    optionIndex,
                    name,
                    value));
        }

        internal static ParsedCommand Empty { get; } = new ParsedCommand(string.Empty);
    }
}
