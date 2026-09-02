using System.Text;
using MC.Code.CLI.Help;
using MC.Code.CLI.FormaterHelp.Base;

namespace MC.Code.CLI.FormaterHelp
{
    public sealed class ApplicationHelpCompactFormatter
        : ApplicationHelpFormatterBase
    {

        public ApplicationHelpCompactFormatter(
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

            foreach (var item in help.Items)
            {
                if (item.Type !=
                    ApplicationHelpItemType.Command)
                {
                    continue;
                }

                output.AppendLine(
                    item.Command);
            }

            return output.ToString();
        }
    }
}