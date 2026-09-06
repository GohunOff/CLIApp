# MC.Code.CLI

**MC.Code.CLI** is a lightweight and extensible command-line interface (CLI) framework for **.NET Framework 4.8** applications.

The library simplifies the creation of console applications by allowing commands, parameters, and options to be defined directly in C# code using attributes.

Instead of manually parsing command-line arguments, validating parameters, dispatching commands, and creating help information, MC.Code.CLI handles these tasks automatically.

## Features

- Attribute-based command definition
- Automatic command discovery
- Command-line argument parsing
- Positional parameters
- Named options
- Multiple options mapped to a single parameter
- Default parameter values
- Default option values
- Automatic command invocation
- Automatic help command
- Command metadata access through `CurrentCommand`
- Simple integration with existing .NET Framework 4.8 applications
- Lightweight and extensible
- No external dependencies

---

## Installation

MC.Code.CLI is distributed as a NuGet package.

### NuGet Package Manager

Install the package using the NuGet Package Manager:

```powershell
Install-Package mcCLIApp
```

### .NET CLI

```bash
dotnet add package mcCLIApp
```

After installing the package, reference the required namespaces:

```csharp
using MC.Code.CLI.Base;
using MC.Code.CLI.Command;
```

### Requirements

- .NET Framework 4.8
- C# or another compatible .NET development environment

MC.Code.CLI does not require additional external dependencies.

---

## Quick Start

A command class inherits from `CliCommand`.

Individual methods are exposed as command-line commands using the `ApplicationCommand` attribute.

### Simple Command

```csharp
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
```

The command can then be invoked from the console:

```text
MyApplication.exe version
```

Output:

```text
Version 1.0.0
```

The command name and description are defined directly in the `ApplicationCommand` attribute.

---

## Application Initialization

MC.Code.CLI is initialized through CLIApp.Init().

A basic application can be configured as follows:

```csharp
static void Main(string[] args)
{
    MC.Code.CLI.CLIApp.Init(
        "program tetowy",
        new MC.Code.CLI.Presentation.ConsoleApplicationInfoOutput(),
        MC.Code.CLI.CLIApp.NoArgsBehavior.ShowHelp,
        args);

    MC.Code.CLI.CLIApp.Run();
}
```

The arguments supplied to CLIApp.Init() define:

Application description
Application information and help presentation
Behavior when no command-line arguments are supplied
Original command-line arguments
CLIApp.Run() starts command discovery, argument processing, and command invocation.

---

## Commands


Commands are defined using the `ApplicationCommand` attribute.

The attribute accepts the command name and description:

```csharp
[ApplicationCommand(
    "version",
    "Displays the application version")]
public void Version()
{
    Console.WriteLine("Version 1.0.0");
}
```

In this example:

- `version` is the command name.
- `Displays the application version` is the command description.
- `Version()` is the method invoked when the command is executed.

The command can be called using:

```text
MyApplication.exe version
```

### Multiple Commands

Multiple commands can be defined in the same command class:

```csharp
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
```

The application can then expose several commands:

```text
MyApplication.exe version
MyApplication.exe config
MyApplication.exe start
```

---

## Help Command

A command can be marked as the application's help command by setting `isHelp` to `true`.

```csharp
[ApplicationCommand(
    "help",
    "Displays application help",
    isHelp: true)]
public void Help()
{
}
```

The `isHelp: true` flag tells MC.Code.CLI that this command represents the application's help functionality.

The help command can then be invoked with:

```text
MyApplication.exe help
```

The framework can use command metadata to present information about available commands, parameters, and options.

The help command itself does not need to contain the help-generation logic.

Its purpose is to identify which command should be treated as the application's help command.

---

## Parameters

Command methods can define positional parameters using the `ApplicationParameter` attribute.

For example:

```csharp
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
```

The parameter is supplied directly after the command name:

```text
MyApplication.exe start automatic
```

The value is passed to the method:

```text
startupMode = automatic
```

Parameters are mapped according to their position in the command line.

### Parameter Descriptions

A parameter can contain a description:

```csharp
[ApplicationParameter(
    description: "Application startup mode")]
string startupMode
```

The description can be used by MC.Code.CLI when generating help information.

This keeps the command-line documentation close to the parameter that it describes.

---

## Default Parameter Values

Parameters can have default values using standard C# optional parameters.

For example:

```csharp
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
```

When the second parameter is not supplied, its default C# value is used:

```text
additionalParameter = default
```

This makes it possible to define commands where some parameters are optional.

---

## Options

Named command-line options are defined using the `ApplicationOption` attribute.

For example:

```csharp
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
```

The option can be specified using its name:

```text
MyApplication.exe start automatic --verbose
```

When `--verbose` is not specified, the default value is used:

```text
verbose = false
```

When the option is specified:

```text
verbose = true
```

---

## Option Syntax
MC.Code.CLI presents command names using the -- prefix:

```text
--start
--version
--config
```

Named options are presented using the / prefix:

```text
/verbose
/fast
/slow
```

For example:

```text
MyApplication.exe --start automatic /fast /verbose
```

---

## Multiple Options for One Parameter

MC.Code.CLI allows multiple `ApplicationOption` attributes to be associated with the same method parameter.

This is useful when several command-line option names represent different values of the same parameter.

For example:

```csharp
public enum ExecutionMode
{
    Slow,
    Fast
}
```

The same parameter can have multiple option names:

```csharp
[ApplicationOption(
    "slow",
    "Run the application in slow mode")]
[ApplicationOption(
    "fast",
    "Run the application in fast mode")]
ExecutionMode executionMode = ExecutionMode.Slow
```

The same parameter can therefore react to multiple option names:

```text
MyApplication.exe start automatic --slow
```

or:

```text
MyApplication.exe start automatic --fast
```

The resulting `ExecutionMode` value can be used by the command implementation to determine how the application should run.

Possible values are:

```text
ExecutionMode.Slow
```

or:

```text
ExecutionMode.Fast
```

This provides a convenient way to expose enum-based application modes through readable command-line options.

---

## Boolean Options

Boolean options are useful for enabling or disabling functionality.

For example:

```csharp
[ApplicationOption(
    "verbose",
    "Enable verbose output")]
bool verbose = false
```

Without the option:

```text
MyApplication.exe start automatic
```

the value is:

```text
verbose = false
```

With the option:

```text
MyApplication.exe start automatic --verbose
```

the value becomes:

```text
verbose = true
```

---

## Generated Application Information and Help Output
MC.Code.CLI provides standard console formatting for both application information and available commands.

When ConsoleApplicationInfoOutput is used, the application can display information about the current application and its execution environment followed by the list of available commands.

For example:

```csharp
static void Main(string[] args)
{
    MC.Code.CLI.CLIApp.Init(
        "program tetowy",
        new MC.Code.CLI.Presentation.ConsoleApplicationInfoOutput(),
        MC.Code.CLI.CLIApp.NoArgsBehavior.ShowHelp,
        args);

    MC.Code.CLI.CLIApp.Run();
}
```

The generated output can look similar to:

```text
**********************************************************
    testApi -> program tetowy
----------------------------------------------------------
    App version        : 1.0.0.0
    Framework          : .NETFramework,Version=v4.8
    Runtime .NET       : .NET Framework 4.8.9345.0
    System             : Microsoft Windows 10.0.26200
    Architecture       : X86
    Launched           : 2026-09-06 13:32:54
    Catalog            : D:\WORK_PZ\_C_SHARP\__MC__\Test\testApi\testApi\bin\Debug\
----------------------------------------------------------
**********************************************************
Available commands:
----------------------------------------------------------
    --config                    -> Configures the application
    --start                     -> Starts the application
      /slow                           +> Run the application in slow mode
      /fast                           +> Run the application in fast mode
      /verbose                        +> Enable verbose output
            %1 additionalParameter            Additional parameter
    --version                   -> Displays the application version
```

The exact values of version, runtime, operating system, architecture, launch time, and application location depend on the environment in which the application is executed.

---

## Complete Example

The following example demonstrates:

- Commands
- Parameters
- Options
- Default values
- Multiple option names
- Access to the current command

```csharp
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
```

---

## Example Commands

### Display Help

```text
MyApplication.exe help
```

### Display the Application Version

```text
MyApplication.exe version
```

### Configure the Application

```text
MyApplication.exe config
```

### Start Using Default Values

```text
MyApplication.exe start automatic
```

### Start in Fast Mode

```text
MyApplication.exe start automatic custom --fast
```

### Start in Fast Mode with Verbose Output

```text
MyApplication.exe start automatic custom --fast --verbose
```

### Start in Slow Mode

```text
MyApplication.exe start automatic custom --slow
```

---

## Example Output

For:

```text
MyApplication.exe start automatic custom --fast --verbose
```

the command receives:

```text
startupMode         = automatic
additionalParameter = custom
executionMode       = Fast
verbose             = true
```

The command implementation can then use these values to perform the required application logic.

---

## Current Command

Command classes inherit from `CliCommand`.

This provides access to information about the currently executed command through the `CurrentCommand` property.

For example:

```csharp
public void Start(
    [ApplicationParameter(
        description: "Application startup mode")]
    string startupMode)
{
    var command = this.CurrentCommand;

    // Access information about the current command.
}
```

This can be useful when command implementations need access to command metadata or execution context.

---

## Attribute Reference

## ApplicationCommand

Defines a command exposed by the application.

Example:

```csharp
[ApplicationCommand(
    "version",
    "Displays the application version")]
public void Version()
{
}
```

The command definition contains:

- Command name
- Command description
- Optional help-command designation using `isHelp`

---

## ApplicationParameter

Defines a positional command-line parameter.

Example:

```csharp
[ApplicationParameter(
    description: "Input file")]
string file
```

Parameters are supplied according to their position in the command line.

---

## ApplicationOption

Defines a named command-line option.

Example:

```csharp
[ApplicationOption(
    "verbose",
    "Enable verbose output")]
bool verbose = false
```

The option can be specified using:

```text
--verbose
```

Multiple `ApplicationOption` attributes can be applied to a single parameter when several option names should map to the same parameter.

---

## Command Discovery

MC.Code.CLI is designed to discover command definitions automatically.

Commands are implemented as methods inside classes derived from `CliCommand`.

The command attributes provide the metadata required by the framework to determine:

- Command names
- Command descriptions
- Parameters
- Parameter descriptions
- Options
- Option descriptions
- Default values
- The help command

This keeps the command-line definition close to the application code.

---

## How It Works

The general execution flow is:

```text
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
```

For example:

```text
MyApplication.exe --start automatic custom /fast /verbose
```

is mapped to a method such as:

```csharp
public void Start(
    string startupMode,
    string additionalParameter,
    ExecutionMode executionMode,
    bool verbose)
```

The framework is responsible for interpreting the command-line arguments and invoking the appropriate method.

The presentation layer can then use the discovered metadata to generate application information and help output.

---

## Why Use MC.Code.CLI?

Traditional command-line applications often require manually handling:

- Argument arrays
- Command selection
- Parameter validation
- Option parsing
- Default values
- Help generation
- Command dispatching

MC.Code.CLI moves this configuration into C# attributes.

Instead of writing command-line parsing code, developers can describe the CLI directly alongside the methods that implement the command.

For example:

```csharp
[ApplicationCommand(
    "version",
    "Displays the application version")]
public void Version()
{
    Console.WriteLine("Version 1.0.0");
}
```

The command definition is immediately visible next to its implementation.

---

## Intended Use

MC.Code.CLI can be used to build many types of command-line applications, including:

- Development and build tools
- File management utilities
- Automation tools
- Administration utilities
- Data processing applications
- Conversion tools
- System utilities
- Internal company tools

---

## Design Goals

The main goals of MC.Code.CLI are:

- Simplicity
- Minimal boilerplate code
- Clear command definitions
- Extensibility
- Easy integration with existing applications

The framework is designed so that command-line behavior can be described close to the code that implements the command.

---

## Requirements

- .NET Framework 4.8
- C# or another compatible .NET development environment

MC.Code.CLI does not require additional external dependencies.

---

## License

This project is licensed under the MIT License.

See [`LICENSE.txt`](LICENSE.txt) for the complete license text.

Copyright (c) 2026 gohunoff@gmail.com

**Author:** Przemysław Załuska  
**Email:** gohunoff@gmail.com

**GitHub:**  [MC.Code.CLI on GitHub]https://github.com/GohunOff/CLIApp

MC.Code.CLI is developed and maintained by the author.

If you find a problem, have a feature request, or would like to contribute, please open an issue in the GitHub repository.
