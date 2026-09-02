using System;
using System.Reflection;

namespace MC.Code.CLI.Command
{
    /// <summary>
    /// Defines a command-line parameter accepted by an application command.
    /// </summary>
    [AttributeUsage(
    AttributeTargets.Parameter,
    AllowMultiple = false)]
    public sealed class ApplicationParameterAttribute
            : Attribute
    {
        private ParameterInfo _parameterInfo;

        private ParameterInfo ParameterInfo =>
            _parameterInfo
            ?? throw new InvalidOperationException(
                "ParameterInfo has not been set.");

        /// <summary>
        /// Gets the description of the parameter.
        /// </summary>
        public string Description { get; }
        /// <summary>
        /// Gets the name of the parameter.
        /// </summary>
        public string Name => ParameterInfo.Name;
        /// <summary>
        /// Gets a value indicating whether the parameter is required.
        /// </summary>
        /// 
        public bool IsRequired => !HasDefaultValue;
        public bool HasDefaultValue => ParameterInfo.HasDefaultValue;
        public object DefaultValue => ParameterInfo.HasDefaultValue
                                        ? ParameterInfo.DefaultValue
                                        : null;
        /// <summary>
        /// Gets the type of the parameter.
        /// </summary>
        public Type ParameterType => ParameterInfo.ParameterType;
        /// <summary>
        /// Gets the one-based index of the parameter.
        /// </summary>
        public int Index => ParameterInfo.Position + 1;

        internal void SetParameterInfo(ParameterInfo parameterInfo)
        {
            _parameterInfo = parameterInfo
                ?? throw new ArgumentNullException(nameof(parameterInfo));
        }

        public ApplicationParameterAttribute(
            string description = "")
        {
            Description = description ?? throw new ArgumentNullException(nameof(description));
        }

        public override string ToString()
        {
            return $"`{Name}` -> " +
                   $"Required:{IsRequired}, " +
                   $"Default:{DefaultValue}";
        }

    }
}
