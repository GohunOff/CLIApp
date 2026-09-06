using MC.Code.CLI.FormaterHelp.Base;
using MC.Code.CLI.Help;
using MC.Code.CLI.RES;
using System.Linq;
using System.Resources;
using System.Text;

namespace MC.Code.CLI.FormaterHelp
{
    public sealed class ApplicationHelpStandardFormatter
        : ApplicationHelpFormatterBase
    {
        private static readonly LocalizedResource _messages
                = new LocalizedResource(new ResourceManager
        ("MC.Code.CLI.FormaterHelp.Resources.ApplicationHelpFormatter",
        typeof(ApplicationHelpStandardFormatter).Assembly));

        public ApplicationHelpStandardFormatter(
           ApplicationHelp format = null)
           : base(format)
        {
        }

        protected override string FormatCore(
            ApplicationHelp help)
        {
            ValidateHelp(help);

            var output =
                new StringBuilder();

            string indent =
                new string(
                    ' ',
                    help.Format.Indent);

            output.AppendLine(_messages.Get("AvailableCommands"));

            output.AppendLine(
                help.Format.SectionSeparator);

            foreach (var item in help.Items)
            {
                if (item.Type !=
                    ApplicationHelpItemType.Command)
                {
                    continue;
                }

                output.AppendLine(
                    $"{indent}" +
                    $"{ARGS.CommandLineParser.commandPrefix}{item.Command.PadRight(help.Format.CommandWidth)}" +
                    $"{help.Format.CommandSeparator}" +
                    $"{item.Description}");

                if (item.Options != null &&
                    item.Options.Any())
                {
                    foreach (var option in item.Options)
                    {
                        output.Append(
                       FormatOptions(
                           option,
                           indent + "  ",
                           help.Format.CommandWidth));

                    }
                }

                if (item.Properties == null ||
                    item.Properties.Count == 0)
                {
                    continue;
                }

                foreach (var property in item.Properties)
                {
                    output.Append(
                        FormatProperty(
                            property,
                            indent + "  ",
                            help.Format.CommandWidth));
                }
            }

            return output.ToString();
        }
    }
}