using System;

namespace MC.Code.CLI.Application
{
    /// <summary>
    /// Defines the formatting options used to display application information.
    /// </summary>
    public sealed class ApplicationInfoFormat
    {
        /// <summary>
        /// Gets the layout used to display application information.
        /// </summary>
        public ApplicationInfoLayout Layout { get; }

        /// <summary>
        /// Gets the number of spaces used for indentation.
        /// </summary>
        public int Indent { get; }

        /// <summary>
        /// Gets the string used as a border around the application information.
        /// </summary>
        public string Border { get; }

        /// <summary>
        /// Gets the string used to separate individual sections of the application information.
        /// </summary>
        public string Separator { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationInfoFormat"/> class.
        /// </summary>
        /// <param name="layout">
        /// The layout used to display application information.
        /// </param>
        /// <param name="indent">
        /// The number of spaces used for indentation. Must be greater than or equal to zero.
        /// </param>
        /// <param name="border">
        /// The string used as a border around the application information.
        /// </param>
        /// <param name="separator">
        /// The string used to separate individual sections of the application information.
        /// </param>
        public ApplicationInfoFormat(
            ApplicationInfoLayout layout =
                ApplicationInfoLayout.Standard,
            int indent = 4,
            string border =
                "**********************************************************",
            string separator =
                "----------------------------------------------------------")
        {
            if (indent < 0)
                throw new ArgumentOutOfRangeException(nameof(indent));

            if (border == null)
                throw new ArgumentNullException(nameof(border));

            if (separator == null)
                throw new ArgumentNullException(nameof(separator));

            Layout = layout;
            Indent = indent;
            Border = border;
            Separator = separator;
        }
    }
}