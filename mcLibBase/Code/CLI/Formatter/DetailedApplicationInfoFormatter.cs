using System.Text;
using MC.Code.CLI.Application;

namespace MC.Code.CLI.Formatter
{
    public sealed class DetailedApplicationInfoFormatter
        : Base.ApplicationInfoFormatterBase
    {
        public DetailedApplicationInfoFormatter(
            ApplicationInfoFormat format = null)
            : base(format)
        {
        }

        protected override string FormatCore(
            ApplicationInfo info)
        {
            var output =
                new StringBuilder();

            output.AppendLine(
                FormatOptions.Border);

            output.AppendLine(
                $"{info.Name} -> " +
                $"{info.Description}");

            output.AppendLine();

            output.AppendLine(
                $"Version       : {info.Version}");

            output.AppendLine(
                $"Framework     : {info.Framework}");

            output.AppendLine(
                $"Runtime       : {info.RuntimeVersion}");

            output.AppendLine(
                $"OS            : {info.OperatingSystem}");

            output.AppendLine(
                $"Architecture  : {info.Architecture}");

            output.AppendLine(
                $"Started       : {info.StartTime:O}");

            output.AppendLine(
                $"Location      : {info.Location}");

            output.Append(
                FormatOptions.Border);

            return output.ToString();
        }
    }
}