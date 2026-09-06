using System.Text;
using MC.Code.CLI.Application;

namespace MC.Code.CLI.Formatter
{
    public sealed class StandardApplicationInfoFormatter
        : Base.ApplicationInfoFormatterBase
    {
        public StandardApplicationInfoFormatter(
            ApplicationInfoFormat format = null)
            : base(format)
        {
        }

        protected override string FormatCore(
            ApplicationInfo info)
        {
            var output =
                new StringBuilder();

            string indent =
                new string(
                    ' ',
                    FormatOptions.Indent);

            output.AppendLine(
                FormatOptions.Border);

            output.AppendLine(
                $"{indent}{info.Name} -> " +
                $"{info.Description}");

            output.AppendLine(
                FormatOptions.Separator);

            AppendLine(
                output,
                "App version",
                info.Version);

            AppendLine(
                output,
                "Framework",
                info.Framework);

            AppendLine(
                output,
                "Runtime .NET",
                info.RuntimeVersion);

            AppendLine(
                output,
                "System",
                info.OperatingSystem);

            AppendLine(
                output,
                "Architecture",
                info.Architecture);

            AppendLine(
                output,
                "Launched",
                info.StartTime.ToString(
                    "yyyy-MM-dd HH:mm:ss"));

            AppendLine(
                output,
                "Catalog",
                info.Location);

            output.AppendLine(
                FormatOptions.Separator);

            output.Append(
                FormatOptions.Border);

            return output.ToString();
        }

        private void AppendLine(
            StringBuilder output,
            string name,
            string value)
        {
            string indent =
                new string(
                    ' ',
                    FormatOptions.Indent);

            output.AppendLine(
                $"{indent}{name,-18} : {value}");
        }
    }
}