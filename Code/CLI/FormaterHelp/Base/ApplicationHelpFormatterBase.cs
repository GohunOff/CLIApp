using MC.Code.CLI.Command;
using MC.Code.CLI.Help;
using System;
using System.Text;

namespace MC.Code.CLI.FormaterHelp.Base
{

    public abstract class ApplicationHelpFormatterBase
        : IApplicationHelpFormatter
    {
        protected ApplicationHelpFormatterBase(
        ApplicationHelp format = null)
        {
            FormatComand =
                format ??
                new ApplicationHelp();
        }


        protected ApplicationHelp FormatComand
        {
            get;
        }


        public string Format(ApplicationHelp help)
        {
            if (help == null)
                throw new ArgumentNullException(
                    nameof(help));

            return FormatCore(help);
        }

        protected void ValidateHelp(
            ApplicationHelp help)
        {
            if (help == null)
                throw new ArgumentNullException(
                    nameof(help));
        }


        protected abstract string FormatCore(
            ApplicationHelp help);

        protected string FormatOptions(ApplicationOptionAttribute options,
            string indent, int commandWidth)
        {
            if (options == null)
                throw new ArgumentNullException(
                    nameof(options));

            var output =
                new StringBuilder();

            if (!string.IsNullOrWhiteSpace(
               options.Name))
            {
                output.Append(indent);
                output.Append(
                    $"{ARGS.CommandLineParser.optionPrefix}{options.Name.PadRight(commandWidth)}");
                output.Append(indent);
                output.Append("+> ");
                output.AppendLine(options.Description);
            }

            return output.ToString();
        }


        protected string FormatProperty(
            ApplicationParameterAttribute property,
            string indent,int commandWidth =10)
        {
            if (property == null)
                throw new ArgumentNullException(
                    nameof(property));

            var output =
                new StringBuilder();

            output.Append(indent);
            output.Append(indent);

            output.Append(
                $"%{property.Index} {property.Name.PadRight(commandWidth)}");

            if (property.IsRequired)
            {
                output.Append(
                    " [required]");
            }

            if (!string.IsNullOrWhiteSpace(
                property.Description))
            {
                output.Append(indent);
                output.AppendLine(
                    property.Description);
            }

            return output.ToString();
        }
    }
}
