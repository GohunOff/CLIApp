using MC.Code.CLI.Application;

namespace MC.Code.CLI.Formatter
{
    public sealed class CompactApplicationInfoFormatter
        : Base.ApplicationInfoFormatterBase
    {
        public CompactApplicationInfoFormatter(
            ApplicationInfoFormat format = null)
            : base(format)
        {
        }

        protected override string FormatCore(
            ApplicationInfo info)
        {
            return
                $"{info.Name} " +
                $"v{info.Version} " +
                $"({info.Architecture})";
        }
    }
}