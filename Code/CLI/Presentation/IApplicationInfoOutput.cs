using MC.Code.CLI.Help;
using MC.Code.CLI.Application;

namespace MC.Code.CLI.Presentation
{
    /// <summary>
    /// Defines an output interface for displaying application information
    /// and command-line help.
    /// </summary>
    public interface IApplicationInfoOutput
    {
        /// <summary>
        /// Displays general information about the application.
        /// </summary>
        /// <param name="info">
        /// The application information to display.
        /// </param>
        void ShowInfo(ApplicationInfo info);
        /// <summary>
        /// Displays command-line help information for the application.
        /// </summary>
        /// <param name="help">
        /// The application help information to display.
        /// </param>
        void ShowHelp(ApplicationHelp help);
    }
}
