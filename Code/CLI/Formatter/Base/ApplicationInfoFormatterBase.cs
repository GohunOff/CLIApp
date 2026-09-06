using System;
using MC.Code.CLI.Application;

namespace MC.Code.CLI.Formatter.Base
{
    /// <summary>
    /// Provides a base implementation for application information formatters.
    ///
    /// The class provides common formatting configuration and input validation.
    /// Concrete formatters implement the actual presentation logic by
    /// overriding the <see cref="FormatCore"/> method.
    /// </summary>
    public abstract class ApplicationInfoFormatterBase
        : IApplicationInfoFormatter
    {
        /// <summary>
        /// Initializes a new instance of the formatter with the specified
        /// formatting configuration.
        /// </summary>
        /// <param name="format">
        /// The formatting configuration.
        /// If not specified, a default <see cref="ApplicationInfoFormat"/>
        /// instance is created.
        /// </param>
        protected ApplicationInfoFormatterBase(
            ApplicationInfoFormat format = null)
        {
            FormatOptions =
                format ??
                new ApplicationInfoFormat();
        }
        /// <summary>
        /// Gets the formatting configuration used by the formatter.
        /// </summary>
        protected ApplicationInfoFormat FormatOptions
        {
            get;
        }
        /// <summary>
        /// Formats the specified application information.
        ///
        /// This method performs common input validation and then delegates
        /// the actual formatting to the concrete formatter implementation.
        /// </summary>
        /// <param name="info">
        /// The application information to format.
        /// </param>
        /// <returns>
        /// The formatted application information.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="info"/> is null.
        /// </exception>
        public string Format(ApplicationInfo info)
        {
            if (info == null)
                throw new ArgumentNullException(
                    nameof(info));

            return FormatCore(info);
        }

        protected abstract string FormatCore(
            ApplicationInfo info);
    }
}