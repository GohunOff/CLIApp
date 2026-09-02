using System;
using System.Collections.Generic;
using System.Reflection;

namespace MC.Code.CLI.Command
{
    /// <summary>
    /// Represents an application command that can be invoked through the CLI.
    /// </summary>
    public sealed class ApplicationCommand
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
        /// Gets the method associated with the command.
        /// </summary>
        public MethodInfo Method { get; }
        /// <summary>
        /// Gets the type that declares the command method.
        /// </summary>
        public Type DeclaringType =>
            Method.DeclaringType;

        public bool IsHelp { get; }
        /// <summary>
        /// Gets the parameters accepted by the command.
        /// </summary>
        public IReadOnlyList<ApplicationParameterAttribute> Parameters { get; }

        public IReadOnlyList<ApplicationOptionAttribute> Options { get; }
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationCommand"/> class.
        /// </summary>
        /// <param name="command">
        /// The name of the command.
        /// </param>
        /// <param name="shortCommand">
        /// The short name of the command.
        /// </param>
        /// <param name="description">
        /// A description of the command.
        /// </param>
        /// <param name="method">
        /// The method associated with the command.
        /// </param>
        /// <param name="parameters">
        /// The parameters accepted by the command.
        /// </param>
        public ApplicationCommand(
            string command,
            string shortCommand,
            string description,
            MethodInfo method,
            IReadOnlyList<ApplicationParameterAttribute> parameters,
            IReadOnlyList<ApplicationOptionAttribute> options,
            bool isHelp = false)
        {
            Command = command ?? throw new ArgumentNullException(nameof(command));
            ShortCommand =
                    shortCommand ??
                    string.Empty;
            Description = description ?? throw new ArgumentNullException(nameof(description));
            Method = method ?? throw new ArgumentNullException(nameof(method));
            Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            Options = options ?? throw new ArgumentNullException(nameof(options));
            IsHelp = isHelp;
        }
    }
}