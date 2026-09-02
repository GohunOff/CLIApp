using System;

namespace MC.Code.CLI.ARGS
{
    public sealed class ParsedOption
    {
        public string Name { get; }
        public string Value { get; }

        public ParsedOption(string name, string value)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
}
