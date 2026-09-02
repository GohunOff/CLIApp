using System.Text;
using MC.Code.CLI.Help;
using MC.Code.CLI.FormaterHelp.Base;

namespace MC.Code.CLI.FormaterHelp
{
    public sealed class ApplicationHelpDetailedFormatter
        : ApplicationHelpFormatterBase
    {

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

            output.AppendLine(
                "Dostępne polecenia:");

            output.AppendLine();

            foreach (var item in help.Items)
            {
                if (item.Type !=
                    ApplicationHelpItemType.Command)
                {
                    continue;
                }

                output.AppendLine(
                    $"Command : {item.Command}");

                output.AppendLine(
                    $"Description : {item.Description}");

                if (item.Properties != null &&
                    item.Properties.Count > 0)
                {
                    output.AppendLine(
                        "Properties:");

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