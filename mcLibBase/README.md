MC.Code.CLI
MC.Code.CLI is a lightweight and extensible command-line interface (CLI) framework for .NET Framework 4.8 applications.

The library simplifies the creation of console applications by allowing commands, parameters, and options to be defined directly in C# code using attributes.

Instead of manually parsing command-line arguments, validating parameters, dispatching commands, and creating help information, MC.Code.CLI handles these tasks automatically.

Features
Attribute-based command definition
Automatic command discovery
Command-line argument parsing
Positional parameters
Named options
Multiple options mapped to a single parameter
Default parameter values
Default option values
Automatic command invocation
Automatic help command
Command metadata access through CurrentCommand
Simple integration with existing .NET Framework 4.8 applications
Lightweight and extensible
No external dependencies
Installation
MC.Code.CLI is distributed as a NuGet package.

Install the package using the NuGet Package Manager:

Install-Package mcCLIApp

Or using the .NET CLI:

dotnet add package mcCLIApp

After installing the package, reference the required namespaces:

using MC.Code.CLI.Base;
using MC.Code.CLI.Command;

The package targets:

.NET Framework 4.8
Quick Start
A command class inherits from CliCommand.

Individual methods are exposed as command-line commands using the ApplicationCommand attribute.

A simple example:

using MC.Code.CLI.Base;
using MC.Code.CLI.Command;
using System;

public class Commands : CliCommand
{
    [ApplicationCommand(
        "version",
        "Displays the application version")]
    public void Version()
    {
        Console.WriteLine("Version 1.0.0");
    }
}

The command can then be invoked from the console:

MyApplication.exe version

Output:

Version 1.0.0

The command name and description are defined directly in the ApplicationCommand attribute.

Commands
Commands are defined using the ApplicationCommand attribute.

The attribute accepts the command name and description:

[ApplicationCommand(
    "version",
    "Displays the application version")]
public void Version()
{
    Console.WriteLine("Version 1.0.0");
}

In this example:

version is the command name.
Displays the application version is the command description.
Version() is the method invoked when the command is executed.
The command can be called using:

MyApplication.exe version

Multiple commands can be defined in the same command class:

public class Commands : CliCommand
{
    [ApplicationCommand(
        "version",
        "Displays the application version")]
    public void Version()
    {
        Console.WriteLine("Version 1.0.0");
    }

    [ApplicationCommand(
        "config",
        "Configures the application")]
    public void Config()
    {
        Console.WriteLine("Configuration");
    }

    [ApplicationCommand(
        "start",
        "Starts the application")]
    public void Start()
    {
        Console.WriteLine("Application started");
    }
}

The application can then expose several commands:

MyApplication.exe version
MyApplication.exe config
MyApplication.exe start

Help Command
A command can be marked as the application's help command by setting isHelp to true.

[ApplicationCommand(
    "help",
    "Displays application help",
    isHelp: true)]
public void Help()
{
}

The isHelp: true flag tells MC.Code.CLI that this command represents the application's help functionality.

The help command can then be invoked with:

MyApplication.exe help

The framework can use command metadata to present information about available commands, parameters, and options.

The help command itself does not need to contain the help-generation logic. Its purpose is to identify which command should be treated as the application's help command.

Parameters
Command methods can define positional parameters using the ApplicationParameter attribute.

For example:

[ApplicationCommand(
    "start",
    "Starts the application")]
public void Start(
    [ApplicationParameter(
        description: "Application startup mode")]
    string startupMode)
{
    Console.WriteLine($"Startup mode: {startupMode}");
}

The parameter is supplied directly after the command name:

MyApplication.exe start automatic

The value is passed to the method:

startupMode = automatic

Parameters are mapped according to their position in the command line.

Parameter Descriptions
A parameter can contain a description:

[ApplicationParameter(
    description: "Application startup mode")]
string startupMode

The description can be used by MC.Code.CLI when generating help information.

This keeps the command-line documentation close to the parameter that it describes.

Default Parameter Values
Parameters can have default values using standard C# optional parameters.

For example:

[ApplicationCommand(
    "start",
    "Starts the application")]
public void Start(
    [ApplicationParameter(
        description: "Application startup mode")]
    string startupMode,

    [ApplicationParameter(
        description: "Additional parameter")]
    string additionalParameter = "default")
{
    Console.WriteLine($"Startup mode: {startupMode}");
    Console.WriteLine($"Additional parameter: {additionalParameter}");
}

When the second parameter is not supplied, its default C# value is used:

additionalParameter = default

This makes it possible to define commands where some parameters are optional.

Options
Named command-line options are defined using the ApplicationOption attribute.

For example:

[ApplicationCommand(
    "start",
    "Starts the application")]
public void Start(
    [ApplicationParameter(
        description: "Application startup mode")]
    string startupMode,

    [ApplicationOption(
        "verbose",
        "Enable verbose output")]
    bool verbose = false)
{
    Console.WriteLine($"Startup mode: {startupMode}");
    Console.WriteLine($"Verbose: {verbose}");
}

The option can be specified using its name:

MyApplication.exe start automatic --verbose

When --verbose is not specified, the default value is used:

verbose = false

When the option is specified:

verbose = true

Multiple Options for One Parameter
MC.Code.CLI allows multiple ApplicationOption attributes to be associated with the same method parameter.

This is useful when several command-line option names represent different values of the same parameter.

For example:

public enum ExecutionMode
{
    Slow,
    Fast
}

[ApplicationOption(
    "slow",
    "Run the application in slow mode")]
[ApplicationOption(
    "fast",
    "Run the application in fast mode")]
ExecutionMode executionMode = ExecutionMode.Slow

The same parameter can therefore react to multiple option names:

MyApplication.exe start automatic --slow

or:

MyApplication.exe start automatic --fast

The resulting ExecutionMode value can be used by the command implementation to determine how the application should run.

For example:

ExecutionMode.Slow

or:

ExecutionMode.Fast

This provides a convenient way to expose enum-based application modes through readable command-line options.

Boolean Options
Boolean options are useful for enabling or disabling functionality.

For example:

[ApplicationOption(
    "verbose",
    "Enable verbose output")]
bool verbose = false

Without the option:

MyApplication.exe start automatic

the value is:

verbose = false

With the option:

MyApplication.exe start automatic --verbose

the value becomes:

verbose = true

Complete Example
The following example demonstrates commands, parameters, options, default values, multiple option names, and access to the current command.

using MC.Code.CLI.Base;
using MC.Code.CLI.Command;
using System;

namespace Test
{
    public enum ExecutionMode
    {
        Slow,
        Fast
    }

    public class Commands : CliCommand
    {
        [ApplicationCommand(
            "help",
            "Displays application help",
            isHelp: true)]
        public void Help()
        {
        }

        [ApplicationCommand(
            "version",
            "Displays the application version")]
        public void Version()
        {
            Console.WriteLine("Version 1.0.0");
        }

        [ApplicationCommand(
            "config",
            "Configures the application")]
        public void Config()
        {
            Console.WriteLine("Configuration");
        }

        [ApplicationCommand(
            "start",
            "Starts the application")]
        public void Start(
            [ApplicationParameter(
                description: "Application startup mode")]
            string startupMode,

            [ApplicationParameter(
                description: "Additional parameter")]
            string additionalParameter = "default",

            [ApplicationOption(
                "slow",
                "Run the application in slow mode")]
            [ApplicationOption(
                "fast",
                "Run the application in fast mode")]
            ExecutionMode executionMode = ExecutionMode.Slow,

            [ApplicationOption(
                "verbose",
                "Enable verbose output")]
            bool verbose = false)
        {
            var command = this.CurrentCommand;

            Console.WriteLine(
                $"Startup mode: {startupMode}");

            Console.WriteLine(
                $"Additional parameter: {additionalParameter}");

            Console.WriteLine(
                $"Execution mode: {executionMode}");

            Console.WriteLine(
                $"Verbose: {verbose}");
        }
    }
}

Example Commands
Display help:

MyApplication.exe help

Display the application version:

MyApplication.exe version

Configure the application:

MyApplication.exe config

Start the application using the default values:

MyApplication.exe start automatic

Start the application in fast mode:

MyApplication.exe start automatic custom --fast

Start the application in fast mode with verbose output:

MyApplication.exe start automatic custom --fast --verbose

Start the application in slow mode:

MyApplication.exe start automatic custom --slow

Example Output
For:

MyApplication.exe start automatic custom --fast --verbose

the command receives:

startupMode        = automatic
additionalParameter = custom
executionMode      = Fast
verbose             = true

The command implementation can then use these values to perform the required application logic.

Current Command
Command classes inherit from CliCommand.

This provides access to information about the currently executed command through the CurrentCommand property.

For example:

public void Start(
    [ApplicationParameter(
        description: "Application startup mode")]
    string startupMode)
{
    var command = this.CurrentCommand;

    // Access information about the current command.
}

This can be useful when command implementations need access to command metadata or execution context.

Attribute Reference
ApplicationCommand
Defines a command exposed by the application.

Example:

[ApplicationCommand(
    "version",
    "Displays the application version")]
public void Version()
{
}

The command definition contains:

command name,
command description,
optional help-command designation using isHelp.
ApplicationParameter
Defines a positional command-line parameter.

Example:

[ApplicationParameter(
    description: "Input file")]
string file

Parameters are supplied according to their position in the command line.

ApplicationOption
Defines a named command-line option.

Example:

[ApplicationOption(
    "verbose",
    "Enable verbose output")]
bool verbose = false

The option can be specified using:

--verbose

Multiple ApplicationOption attributes can be applied to a single parameter when several option names should map to the same parameter.

Command Discovery
MC.Code.CLI is designed to discover command definitions automatically.

Commands are implemented as methods inside classes derived from CliCommand.

The command attributes provide the metadata required by the framework to determine:

command names,
command descriptions,
parameters,
parameter descriptions,
options,
option descriptions,
default values,
the help command.
This keeps the command-line definition close to the application code.

How It Works
The general execution flow is:

Command line
     |
     v
MC.Code.CLI
     |
     v
Command discovery
     |
     v
Argument parsing
     |
     v
Parameter and option mapping
     |
     v
Command method invocation

For example:

MyApplication.exe start automatic custom --fast --verbose

is mapped to a method such as:

public void Start(
    string startupMode,
    string additionalParameter,
    ExecutionMode executionMode,
    bool verbose)

The framework is responsible for interpreting the command-line arguments and invoking the appropriate method.

Why Use MC.Code.CLI?
Traditional command-line applications often require manually handling:

argument arrays,
command selection,
parameter validation,
option parsing,
default values,
help generation,
command dispatching.
MC.Code.CLI moves this configuration into C# attributes.

Instead of writing command-line parsing code, developers can describe the CLI directly alongside the methods that implement the command.

For example:

[ApplicationCommand(
    "version",
    "Displays the application version")]
public void Version()
{
    Console.WriteLine("Version 1.0.0");
}

The command definition is immediately visible next to its implementation.

Intended Use
MC.Code.CLI can be used to build many types of command-line applications, including:

development and build tools,
file management utilities,
automation tools,
administration utilities,
data processing applications,
conversion tools,
system utilities,
internal company tools.
Design Goals
The main goals of MC.Code.CLI are:

simplicity,
minimal boilerplate code,
clear command definitions,
extensibility,
easy integration with existing applications.
The framework is designed so that command-line behavior can be described close to the code that implements the command.

Requirements
.NET Framework 4.8
C# or another compatible .NET development environment
MC.Code.CLI does not require additional external dependencies.

License
This project is licensed under the MIT License.

See LICENSE.txt for the complete license text.

Copyright (c) 2026 gohunoff@gmail.com

Author: Przemysław Załuska
Email: gohunoff@gmail.com

GitHub: https://github.com/GohunOff/CLIApp

MC.Code.CLI is developed and maintained by the author.

If you find a problem, have a feature request, or would like to contribute, please open an issue in the GitHub repository.