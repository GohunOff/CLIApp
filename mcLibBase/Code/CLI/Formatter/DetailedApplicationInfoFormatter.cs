using MC.Code.CLI.Application;
using MC.Code.CLI.RES;
using System.Resources;
using System.Text;

namespace MC.Code.CLI.Formatter
{
    public sealed class DetailedApplicationInfoFormatter
        : Base.ApplicationInfoFormatterBase
    {
        private static readonly LocalizedResource _messages
        = new LocalizedResource(new ResourceManager
                    ("MC.Code.CLI.Application.Resources.ApplicationInfo",
                    typeof(DetailedApplicationInfoFormatter).Assembly));

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
                $"{_messages.Get("ApplicationVersion"),-14}: " +
                $"{info.Version}");

            output.AppendLine(
                $"{_messages.Get("TargetFramework"),-14}: " +
                $"{info.Framework}");

            output.AppendLine(
                $"{_messages.Get("RuntimeVersion"),-14}: " +
                $"{info.RuntimeVersion}");

            output.AppendLine(
                $"{_messages.Get("OperatingSystem"),-14}: " +
                $"{info.OperatingSystem}");

            output.AppendLine(
                $"{_messages.Get("Architecture"),-14}: " +
                $"{info.Architecture}");

            output.AppendLine(
                $"{_messages.Get("StartTime"),-14}: " +
                $"{info.StartTime:O}");

            output.AppendLine(
                $"{_messages.Get("Location"),-14}: " +
                $"{info.Location}");

            output.Append(
                FormatOptions.Border);

            return output.ToString();
        }
    }
}