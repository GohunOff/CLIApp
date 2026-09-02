using MC.Code.CLI.Command;
using System;
using System.Reflection;

namespace MC.Code.CLI
{
    public sealed class ApplicationCommandParameter
    {
        public ApplicationParameterAttribute Attribute { get; }

        public ParameterInfo Parameter { get; }

        public string Name =>
            Attribute.Name;

        public string Description =>
            Attribute.Description;

        public bool Required =>
            Attribute.IsRequired;

        public Type ParameterType =>
            Parameter.ParameterType;

        public string ParameterName =>
            Parameter.Name;

        public ApplicationCommandParameter(
            ApplicationParameterAttribute attribute,
            ParameterInfo parameter)
        {
            Attribute = attribute
                ?? throw new ArgumentNullException(nameof(attribute));

            Parameter = parameter
                ?? throw new ArgumentNullException(nameof(parameter));
        }
    }
}
