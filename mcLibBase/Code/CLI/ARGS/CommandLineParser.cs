using System;

namespace MC.Code.CLI.ARGS
{
    public static class CommandLineParser
    {
        private static bool _initialized = false;
        private static string _commandPrefix = "--";
        private static string _optionPrefix = "/";

        public static string commandPrefix => _commandPrefix;
        public static string optionPrefix => _optionPrefix;


        private static bool ContainsWhiteSpace(string value)
        {
            for (int i = 0; i < value.Length; i++)
            {
                if (char.IsWhiteSpace(value[i]))
                    return true;
            }

            return false;
        }

        public static void Configure(string commandPrefix,string optionPrefix)
        {
            if (_initialized)
                throw new InvalidOperationException(
                    "CommandLineParser został już zainicjalizowany.");

            if (string.IsNullOrWhiteSpace(commandPrefix))
                throw new ArgumentException(
                    "Prefiks komendy nie może być pusty.",
                    nameof(commandPrefix));

            if (string.IsNullOrWhiteSpace(optionPrefix))
                throw new ArgumentException(
                    "Prefiks opcji nie może być pusty.",
                    nameof(optionPrefix));

            if (ContainsWhiteSpace(commandPrefix))
                throw new ArgumentException(
                    "Prefiks komendy nie może zawierać białych znaków.",
                    nameof(commandPrefix));

            if (ContainsWhiteSpace(optionPrefix))
                throw new ArgumentException(
                    "Prefiks opcji nie może zawierać białych znaków.",
                    nameof(optionPrefix));


            _commandPrefix = commandPrefix;
            _optionPrefix = optionPrefix;

            _initialized = true;
        }

        private static bool IsCommand(string argument)
        {
            if (string.IsNullOrWhiteSpace(argument))
                return false;

            return argument.StartsWith(
                _commandPrefix,
                StringComparison.Ordinal);
        }

        private static bool IsOption(string argument)
        {
            if (string.IsNullOrWhiteSpace(argument))
                return false;

            return argument.StartsWith(
                _optionPrefix,
                StringComparison.Ordinal);
        }

        public static ParsedCommand Parse(string[] args)
        {
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            if (args.Length == 0)
                return ParsedCommand.Empty;

            // =========================
            // KOMENDA MUSI BYĆ PIERWSZA
            // =========================

            string commandArgument = args[0];

            if (!IsCommand(commandArgument))
            {
                throw new ArgumentException(
                    string.Format(
                        "Pierwszy argument musi być komendą rozpoczynającą się od '{0}'.",
                        _commandPrefix),
                    nameof(args));
            }

            string commandName =
                commandArgument.Substring(_commandPrefix.Length);

            if (string.IsNullOrWhiteSpace(commandName))
            {
                throw new ArgumentException(
                    string.Format(
                        "Nieprawidłowa komenda: '{0}'.",
                        commandArgument),
                    nameof(args));
            }

            var result =
                ParsedCommand.Create(commandName);

            // =========================
            // ARGUMENTY I OPCJE
            // =========================
            int position = 0;
            int argumentIndex = 0;
            int optionIndex = 0;

            for (int i = 1; i < args.Length; i++)
            {
                string arg = args[i];

                if (string.IsNullOrWhiteSpace(arg))
                {
                    throw new ArgumentException(
                        "Argument nie może być pusty.",
                        nameof(args));
                }

                // =========================
                // ARGUMENT KOMENDY
                // =========================

                if (!IsOption(arg))
                {
                    position++;
                    argumentIndex++;

                    result.AddArgument(
                        position,
                        argumentIndex,
                        arg);

                    continue;
                }

                // =========================
                // OPCJA
                // =========================

                string optionArgument =
                    arg.Substring(_optionPrefix.Length);

                if (string.IsNullOrWhiteSpace(optionArgument))
                {
                    throw new ArgumentException(
                        string.Format(
                            "Nieprawidłowa opcja: '{0}'.",
                            arg),
                        nameof(args));
                }

                string optionName;
                string value = null;

                // =========================
                // /o=plik.txt
                // =========================

                int equalIndex =
                    optionArgument.IndexOf('=');

                if (equalIndex >= 0)
                {
                    optionName =
                        optionArgument.Substring(0, equalIndex);

                    value =
                        optionArgument.Substring(equalIndex + 1);
                }
                else
                {
                    optionName = optionArgument;
                }

                if (string.IsNullOrWhiteSpace(optionName))
                {
                    throw new ArgumentException(
                        string.Format(
                            "Nieprawidłowa nazwa opcji: '{0}'.",
                            arg),
                        nameof(args));
                }

                optionIndex++;
                position++;

                result.AddOption(
                    position,
                    optionIndex,
                    optionName,
                    value);
            }

            return result;
        }

    }
}
