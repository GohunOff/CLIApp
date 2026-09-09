using MC.Code.CLI.Command;
using System.Collections.Generic;
using System.Reflection;

namespace MC.Code.CLI.Help
{
    public sealed class CliCommandDefinition
    {
        public ApplicationHelpItemType Type { get; }
        public string Command { get; }
        public string Description { get; }
        public IReadOnlyList<ApplicationParameterAttribute> Properties { get; }
        public IReadOnlyList<ApplicationOptionAttribute> Options { get; }
        public bool IsHelp { get; }

        internal MethodInfo Method { get; }

        private CliCommandDefinition(
            ApplicationHelpItemType type,
            string command = null,
            string description = null,
            IReadOnlyList<ApplicationParameterAttribute> properties = null,
            IReadOnlyList<ApplicationOptionAttribute> options = null,
            MethodInfo method = null,
            bool isHelp = false)
        {
            Type = type;
            Command = command;
            Description = description;
            Properties = properties;
            Options = options;
            Method = method;
            IsHelp = isHelp;
        }

        public static CliCommandDefinition CommandItem(
            string command,
            string description,
            IReadOnlyList<ApplicationParameterAttribute> properties,
            IReadOnlyList<ApplicationOptionAttribute> options,
            MethodInfo method = null,
            bool isHelp = false)
        {
            return new CliCommandDefinition(
                ApplicationHelpItemType.Command,
                command,
                description,
                properties,
                options,
                method,
                isHelp);
        }
        public static CliCommandDefinition Header(
            string description)
        {
            return new CliCommandDefinition(
                ApplicationHelpItemType.Header,
                description: description);
        }
        public static CliCommandDefinition Separator()
        {
            return new CliCommandDefinition(
                ApplicationHelpItemType.Separator);
        }

        public static CliCommandDefinition EmptyLine()
        {
            return new CliCommandDefinition(
                ApplicationHelpItemType.EmptyLine);
        }
        public override string ToString()
        {
            return $"`{Command}` -> `{Description}` -> options ({Options?.Count}) prop ({Properties?.Count})";
        }
    }
}
