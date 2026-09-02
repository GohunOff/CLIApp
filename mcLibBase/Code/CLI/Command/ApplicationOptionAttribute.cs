using System;
using System.Reflection;

namespace MC.Code.CLI.Command
{
    /// <summary>
    /// Defines an application option for a command.
    /// </summary>
    [AttributeUsage(
        AttributeTargets.Parameter,
        AllowMultiple = true)]
    public sealed class ApplicationOptionAttribute
        : Attribute
    {
        private ParameterInfo _parameterInfo;
        private ParameterInfo ParameterInfo =>
            _parameterInfo
            ?? throw new InvalidOperationException(
                "ParameterInfo has not been set.");

        /// <summary>
        /// Gets the name of the option.
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Gets the short name of the option.
        /// </summary>
        public string ShortName { get; }

        /// <summary>
        /// Gets the description of the option.
        /// </summary>
        public string Description { get; }

        public bool IsRequired => !HasDefaultValue;
        public bool HasDefaultValue => ParameterInfo.HasDefaultValue;
        public object DefaultValue => ParameterInfo.HasDefaultValue
                                        ? ParameterInfo.DefaultValue
                                        : null;
        public bool IsHelp { get; }
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


        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="ApplicationOptionAttribute"/> class.
        /// </summary>
        /// <param name="name">
        /// The name of the option.
        /// </param>
        /// <param name="description">
        /// The description of the option.
        /// </param>
        public ApplicationOptionAttribute(
            string name,
            string description,
            string shortName = "")
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException(
                    "Option cannot be null or empty.",
                    nameof(name));

            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException(
                    "Description cannot be null or empty.",
                    nameof(description));

            Name = name;
            Description = description;
            ShortName = string.IsNullOrWhiteSpace(shortName)
                    ? null
                    : shortName;
        }

        public override string ToString()
        {
            return $"Option `{Name}` " +
                   $"ShortName:`{ShortName}` " +
                   $"({Index}) -> '{Description}'";
        }
    }
}
