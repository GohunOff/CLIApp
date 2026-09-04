using MC.Code.CLI.FormaterHelp.Base;
using MC.Code.CLI.Help;
using MC.Code.CLI.RES;
using System.Resources;
using System.Text;

namespace MC.Code.CLI.FormaterHelp
{
    public sealed class ApplicationHelpDetailedFormatter
        : ApplicationHelpFormatterBase
    {
        private static readonly LocalizedResource _messages
        = new LocalizedResource(new ResourceManager
            ("MC.Code.CLI.FormaterHelp.Resources.ApplicationHelpFormatter.pl",
            typeof(ApplicationHelpDetailedFormatter).Assembly));

        public ApplicationHelpDetailedFormatter(
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

            output.AppendLine(_messages.Get("Properties"));

            output.AppendLine();

            foreach (var item in help.Items)
            {
                if (item.Type !=
                    ApplicationHelpItemType.Command)
                {
                    continue;
                }

                output.AppendLine(
                    $"{_messages.Get("Command")} : {item.Command}");

                output.AppendLine(
                    $"{_messages.Get("Description")} : {item.Description}");

                if (item.Properties != null &&
                    item.Properties.Count > 0)
                {
                    output.AppendLine(
                        $"{_messages.Get("Properties")}:");

                    foreach (var property in item.Properties)
                    {
                        output.AppendLine(
                            FormatProperty(
                                property,
                                "  "));
                    }
                }

                output.AppendLine();
            }

            return output.ToString();
        }
    }
}