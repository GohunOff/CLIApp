using MC.Code.CLI.RES;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;


namespace MC.Code.CLI.Application
{
    /// <summary>
    /// Provides application build and runtime environment information.
    /// 
    /// The library supports both legacy and modern .NET environments and is
    /// designed to maintain compatibility with C# 7.3. It uses broadly
    /// compatible .NET APIs where possible to support applications running
    /// across different .NET Framework and .NET runtime versions.
    /// 
    /// The library provides information about the application name,
    /// description, version, target framework, runtime environment,
    /// operating system, process architecture, process start time
    /// and application location.
    /// </summary>
    public sealed class ApplicationInfo
    {
        private static readonly LocalizedResource _messages
                = new LocalizedResource(new ResourceManager
        ("MC.Code.CLI.Application.Resources.ApplicationInfo",
        typeof(ApplicationInfo).Assembly));
        // Build information
        /// <summary> 
        /// Gets the application name.
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Gets the description of the application. 
        /// </summary>
        public string Description { get; private set; }
        /// <summary>
        /// Gets the informational version of the application.
        /// </summary>
        public string Version { get; private set; }
        /// <summary>
        /// Gets the target framework of the application.
        /// </summary>
        public string Framework { get; private set; }
        // Runtime information
        /// <summary>
        /// Gets the runtime description of the environment.
        /// </summary>
        public string RuntimeVersion { get; private set; }
        /// <summary>
        /// Gets the operating system description.
        /// </summary>
        public string OperatingSystem { get; private set; }
        /// <summary>
        /// Gets the architecture of the current process.
        /// </summary>
        public string Architecture { get; private set; }
        /// <summary>
        /// Gets the date and time when the current process started.
        /// </summary>
        public DateTime StartTime { get; private set; }
        /// <summary>
        /// Gets the base directory of the running application.
        /// </summary>
        public string Location { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationInfo"/> class.
        /// </summary>
        /// <param name="description">
        /// A description of the application.
        /// </param>
        public ApplicationInfo(string description)
        {
            Description = description;

            Load();
        }

        private void Load()
        {
            Assembly assembly =
                Assembly.GetEntryAssembly();

            Name = assembly?.GetName().Name ?? _messages.Get("UnknownApplication");

            Version = assembly?
                .GetCustomAttribute
                <AssemblyInformationalVersionAttribute>()?
                .InformationalVersion
                ?? assembly?.GetName().Version?.ToString()
                ?? _messages.Get("UnknownVersion");
            
            Framework = assembly?
                .GetCustomAttribute
                <TargetFrameworkAttribute>()?
                .FrameworkName
                ?? _messages.Get("UnknownFramework");

            RuntimeVersion =
                RuntimeInformation.FrameworkDescription;

            OperatingSystem =
                RuntimeInformation.OSDescription;

            Architecture =
                RuntimeInformation.ProcessArchitecture.ToString();

            StartTime = Process.GetCurrentProcess().StartTime;

            Location =
                AppDomain.CurrentDomain.BaseDirectory;
        }
    }
}


