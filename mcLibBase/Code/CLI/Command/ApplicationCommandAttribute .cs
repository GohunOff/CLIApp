using System;

namespace MC.Code.CLI.Command
{
    /// <summary>
    /// Marks a method as an application command.
    /// </summary>
    [AttributeUsage(
        AttributeTargets.Method,
        AllowMultiple = false)]
    public sealed class ApplicationCommandAttribute
        : Attribute
    {
        /// <summary>
        /// Gets the name of the command.
        /// </summary>
        public string Command { get; }
        /// <summary>
        /// Gets the short name of the command.
        /// </summary>
        public string ShortCommand { get; }
        /// <summary>
        /// Gets the description of the command.
        /// </summary>
        public string Description { get; }
        /// <summary>
        /// Gets a value indicating whether this command represents the help command.
        /// </summary>
        public bool IsHelp { get; }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="ApplicationCommandAttribute"/> class.
        /// </summary>
        /// <param name="command">
        /// The name of the command.
        /// </param>
        /// <param name="description">
        /// The description of the command.
        /// </param>
        public ApplicationCommandAttribute(
            string command,
            string description,
            string shortCommand = "",
            bool isHelp = false)
        {
            if (string.IsNullOrWhiteSpace(command))
                throw new ArgumentException(
                    "Command cannot be null or empty.",
                    nameof(command));

            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException(
                    "Description cannot be null or empty.",
                    nameof(description));
            ShortCommand = string.IsNullOrWhiteSpace(shortCommand)
                ? null
                : shortCommand;
            Command = command;
            Description = description;
            IsHelp = isHelp;
        }

        public override string ToString()
        {
            return $"Command `{Command}` " +
                   $"ShortCommand:`{ShortCommand}` " +
                   $"-> '{Description}'";
        }
    }
}
