using MC.Code.CLI.RES;
using System;
using System.Resources;

namespace MC.Code.CLI.ARGS
{
    public sealed class CommandLineArgument
    {
        private static readonly LocalizedResource _messages
            = new LocalizedResource(new ResourceManager
                ("MC.Code.CLI.ARGS.Resources.CommandLineParser",
                typeof(CommandLineArgument).Assembly));

        public int Position { get; }

        public int ArgumentIndex { get; }

        public int OptionIndex { get; }

        public bool IsOption { get; }

        public string Name { get; }

        public string Value { get; }

        private CommandLineArgument(
            int position,
            int argumentIndex,
            int optionIndex,
            bool isOption,
            string name,
            string value)
        {
            Position = position;
            ArgumentIndex = argumentIndex;
            OptionIndex = optionIndex;
            IsOption = isOption;
            Name = name;
            Value = value;
        }

        internal static CommandLineArgument CreateArgument(
            int position,
            int argumentIndex,
            string value)
        {          
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException(_messages.Get(
                                    "ArgumentCannotBeEmpty"),
                    nameof(value));

            if (position <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(position));

            if (argumentIndex <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(argumentIndex));

            return new CommandLineArgument(
                position,
                argumentIndex,
                0,
                false,
                null,
                value);
        }

        internal static CommandLineArgument CreateOption(
            int position,
            int optionIndex,
            string name,
            string value)
        {

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException(_messages.Get(
                                    "OptionNameCannotBeEmpty"),
                    nameof(name));

            if (position <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(position));

            if (optionIndex <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(optionIndex));

            return new CommandLineArgument(
                position,
                0,
                optionIndex,
                true,
                name,
                value);
        }

        public override string ToString()
        {
            return string.Format(
                "#{0} {1} -> {2} : {3}",
                Position,
                IsOption ? "option" : "parametr",
                IsOption ? Name : Value,
                IsOption ? OptionIndex : ArgumentIndex);
        }
    }
}
