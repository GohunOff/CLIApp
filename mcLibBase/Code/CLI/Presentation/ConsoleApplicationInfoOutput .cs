using System;
using MC.Code.CLI.Application;
using MC.Code.CLI.Formatter.Base;
using MC.Code.CLI.FormaterHelp.Base;
using MC.Code.CLI.Help;


namespace MC.Code.CLI.Presentation
{
    public class ConsoleApplicationInfoOutput : IApplicationInfoOutput
    {
        private readonly IApplicationInfoFormatter _infoFormatter;
        private readonly IApplicationHelpFormatter _helpFormatter;

        public ConsoleApplicationInfoOutput(IApplicationInfoFormatter infoFormatter = null, IApplicationHelpFormatter helpFormatter= null)
        {
            _infoFormatter = infoFormatter
                ?? new Formatter.StandardApplicationInfoFormatter();

            _helpFormatter = helpFormatter
                ?? new FormaterHelp.ApplicationHelpStandardFormatter();
        }

        public void ShowHelp(ApplicationHelp help)
        {
            Console.WriteLine(
                _helpFormatter.Format(help));
        }

        public void ShowInfo(ApplicationInfo info)
        {
            Console.WriteLine(
                _infoFormatter.Format(info));
        }
    }
}
