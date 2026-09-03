MC.Code.CLI
MC.Code.CLI is a lightweight and extensible command-line interface (CLI) framework for .NET Framework 4.8 applications.

The library simplifies the creation of console applications by allowing commands, parameters and options to be defined directly in C# code using attributes.

Instead of manually parsing command-line arguments, validating parameters and generating help messages, MC.Code.CLI handles these tasks automatically.

Features
Attribute-based command definition
Automatic command discovery
Command-line argument parsing
Named options and parameters
Default parameter values
Automatic command invocation
Automatic help generation
Simple integration with existing .NET Framework 4.8 applications
Lightweight and easy to extend
No external dependencies
How it works
Commands are defined using C# attributes.

The library discovers classes and methods that represent CLI commands and uses their attributes to determine:

command names,
parameters,
options,
default values,
descriptions,
help information.
The command-line arguments are then parsed and the appropriate method is invoked automatically.

This allows application code to focus on the actual command implementation instead of argument parsing.

Example
A simple command can be defined using attributes:

using MC.Code.CLI;

public class Commands
{
    [Command("hello", Description = "Displays a greeting message.")]
    public void Hello(
        [Parameter("name", Description = "Name of the user.")]
        string name)
    {
        Console.WriteLine($"Hello, {name}!");
    }
}
The command can then be executed from the console:

MyApplication.exe hello John
Result:

Hello, John!
Command-line help
The framework can automatically generate help information based on the commands and their attributes.

For example:

MyApplication.exe help
can display information about available commands, parameters and options.

This makes it easier for users to understand how to use the application without requiring a separate help system.

Parameters and options
Commands can define parameters and options that are automatically recognized by the framework.

Example:

[Command("copy", Description = "Copies a file.")]
public void Copy(
    [Parameter("source")]
    string source,

    [Parameter("destination")]
    string destination,

    [Option("overwrite", Description = "Overwrite the destination file.")]
    bool overwrite = false)
{
    // Command implementation
}
Example usage:

MyApplication.exe copy file.txt backup.txt
or:

MyApplication.exe copy file.txt backup.txt --overwrite
Default values
Parameters and options can have default values.

This allows commands to define optional behavior without requiring the user to specify every argument.

Example:

[Option("verbose")]
bool verbose = false
If --verbose is not specified, the default value is used.

Installation
The library is distributed as a NuGet package.

After installing the package, reference the required namespace:

using MC.Code.CLI;
The package targets:

.NET Framework 4.8
Requirements
.NET Framework 4.8
C# / compatible .NET development environment
The library does not require additional external dependencies.

Intended use
MC.Code.CLI can be used to build many types of command-line applications, including:

development and build tools,
file management utilities,
automation tools,
administration utilities,
data processing applications,
conversion tools,
system utilities,
internal company tools.
Design goals
The main goals of MC.Code.CLI are:

simplicity,
minimal boilerplate code,
clear command definitions,
extensibility,
easy integration with existing applications.
The framework is designed so that command-line behavior can be described close to the code that implements the command.

License
MIT License
Copyright (c) 2026 gohunoff@gmail.com

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

Author
gohunoff@gmail.com

MC.Code.CLI is developed and maintained by the author.

If you find a problem, have an