namespace MC.Code.CLI.Help
{
    public sealed class ApplicationHelpFormat
    {
        public int Indent { get; }

        public int CommandWidth { get; }

        public string CommandSeparator { get; }

        public string SectionSeparator { get; }

        public ApplicationHelpFormat(
            int indent = 4,
            int commandWidth = 25,
            string commandSeparator = " -> ",
            string sectionSeparator =
                "----------------------------------------------------------")
        {
            Indent = indent;
            CommandWidth = commandWidth;
            CommandSeparator = commandSeparator;
            SectionSeparator = sectionSeparator;
        }
    }
}
