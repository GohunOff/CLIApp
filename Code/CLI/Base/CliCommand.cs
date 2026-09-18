using MC.Code.CLI.Command;
using MC.Code.CLI.Help;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace MC.Code.CLI.Base
{
    public abstract class CliCommand
    {
        private const BindingFlags CommandBindingFlags =
        BindingFlags.Public |
        BindingFlags.NonPublic |
        BindingFlags.Instance |
        BindingFlags.Static;

        private static void ValidateCommandMethod(
            Type declaringType,
            MethodInfo method)
        {
            if (!typeof(CliCommand).IsAssignableFrom(declaringType))
            {
                throw new InvalidOperationException(
                    $"'{method.DeclaringType?.FullName}.{method.Name}' " +
                    $"is marked as ApplicationCommand, but its class " +
                    $"does not inherit from CliCommand.");
            }
        }
        private static readonly Lazy<CommandDiscoveryResult> _commands =
            new Lazy<CommandDiscoveryResult>(LoadCliCommandsInternal);
        private static CommandDiscoveryResult LoadCliCommandsInternal()
        {
            var assembly = Assembly.GetEntryAssembly();

            if (assembly == null)
            {
                return new CommandDiscoveryResult(
                    Array.Empty<CliCommandDefinition>(),
                    null);
            }

            return DiscoverCommands(assembly);
        }
        private static CliCommandDefinition CreateCommandDefinition(
     MethodInfo method)
        {
            var attribute =
                method.GetCustomAttribute<ApplicationCommandAttribute>();

            if (attribute == null)
            {
                throw new InvalidOperationException(
                    $"Method '{method.DeclaringType?.FullName}.{method.Name}' " +
                    $"does not have the ApplicationCommandAttribute.");
            }

            var parameters = GetParameters(method);
            var options = GetOptions(method);

            return CliCommandDefinition.CommandItem(
                attribute.Command,
                attribute.Description,
                parameters,
                options,
                method,
                attribute.IsHelp);
        }

        private static bool IsCommandType(Type type)
        {
            return typeof(CliCommand).IsAssignableFrom(type)
                   && !type.IsAbstract;
        }
        private static IEnumerable<MethodInfo> GetCommandMethods(
            Type type)
        {
            var methods = type.GetMethods(CommandBindingFlags);

            foreach (var method in methods)
            {
                if (!HasCommandAttribute(method))
                    continue;

                ValidateCommandAttributes(method);
                ValidateCommandMethod(type, method);

                yield return method;
            }
        }
        private static void ValidateCommandAttributes(MethodInfo method)
        {
            var hasApplicationCommand =
                method.IsDefined(
                    typeof(ApplicationCommandAttribute),
                    inherit: false);

            var hasDefaultCommand =
                method.IsDefined(
                    typeof(DefaultCommandAttribute),
                    inherit: false);

            if (hasApplicationCommand && hasDefaultCommand)
            {
                throw new InvalidOperationException(
                    $"Method '{method.DeclaringType?.FullName}.{method.Name}' " +
                    $"cannot have both {nameof(ApplicationCommandAttribute)} " +
                    $"and {nameof(DefaultCommandAttribute)}.");
            }
        }
        private static bool HasCommandAttribute(MethodInfo method)
        {
            return method.IsDefined(
                typeof(ApplicationCommandAttribute),
                inherit: false)
              || method.IsDefined(
               typeof(DefaultCommandAttribute),
               inherit: false); ;
        }
        private static List<ApplicationParameterAttribute> GetParameters(
            MethodInfo method)
        {
            var result = new List<ApplicationParameterAttribute>();

            foreach (var parameter in method.GetParameters())
            {
                var attribute =
                    parameter.GetCustomAttribute<ApplicationParameterAttribute>();

                if (attribute == null)
                    continue;

                attribute.SetParameterInfo(parameter);

                result.Add(attribute);
            }

            return result;
        }
        private static List<ApplicationOptionAttribute> GetOptions(
            MethodInfo method)
        {
            var result = new List<ApplicationOptionAttribute>();

            foreach (var parameter in method.GetParameters())
            {
                var attributes =
                     parameter.GetCustomAttributes<
                         ApplicationOptionAttribute>();

                foreach (var attribute in attributes)
                {
                    attribute.SetParameterInfo(parameter);

                    result.Add(attribute);
                }
            }
            return result;
        }

        public static IReadOnlyList<CliCommandDefinition> LoadCliCommands
            => _commands.Value.Commands;

        public static CommandDiscoveryResult DiscoverCommands(
            Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            var commandTypes = assembly
                .GetTypes()
                .Where(IsCommandType)
                .ToList();

            if (commandTypes.Count == 0)
            {
                throw new InvalidOperationException(
                    $"No concrete class inheriting from " +
                    $"{nameof(CliCommand)} was found.");
            }

            if (commandTypes.Count > 1)
            {
                var types = string.Join(
                    ", ",
                    commandTypes.Select(x => x.FullName));

                throw new InvalidOperationException(
                    $"Only one class can inherit from " +
                    $"{nameof(CliCommand)}. " +
                    $"Found {commandTypes.Count}: {types}.");
            }

            var commandType = commandTypes[0];

            var commandMethods =
                GetCommandMethods(commandType).ToList();

            if (commandMethods.Count == 0)
            {
                throw new InvalidOperationException(
                    $"Class '{commandType.FullName}' must contain at least " +
                    $"one {nameof(ApplicationCommandAttribute)} or " +
                    $"{nameof(DefaultCommandAttribute)}.");
            }

            var commands =
                new List<CliCommandDefinition>();

            DefaultCommand defaultCommand = null;

            foreach (var method in commandMethods)
            {
                if (method.IsDefined(
                    typeof(DefaultCommandAttribute),
                    inherit: false))
                {
                    if (method.GetParameters().Length > 0)
                    {
                        throw new InvalidOperationException(
                            $"DefaultCommand '{method.Name}' cannot have parameters.");
                    }

                    if (defaultCommand != null)
                    {
                        throw new InvalidOperationException(
                            "Only one DefaultCommand can be defined.");
                    }

                    var instance =
                        Activator.CreateInstance(commandType);

                    defaultCommand = new DefaultCommand(
                        method,
                        (CliCommand)instance);

                    continue;
                }

                commands.Add(
                    CreateCommandDefinition(method));
            }

            return new CommandDiscoveryResult(
                commands
                    .OrderBy(x => x.Command)
                    .ToList(),
                defaultCommand);
        }
        
        public static CliCommandDefinition GetCliCommand(
            MethodBase method)
        {
            if (method == null)
                throw new ArgumentNullException(nameof(method));

            return LoadCliCommands
                .FirstOrDefault(x => x.Method == method);
        }

        //private CliCommandContext _context;

        //public CliCommandContext Context
        //{
        //    get
        //    {
        //        return _context
        //            ?? throw new InvalidOperationException(
        //                "Command context is not available. " +
        //                "The command must be executed by the CLI framework.");
        //    }

        //    internal set
        //    {
        //        _context = value;
        //    }
        //}

        //public CliCommandDefinition CurrentCommand
        //    => Context.Command;
        public CliCommandDefinition CurrentCommand
        {
            get
            {
                var method = new StackFrame(1).GetMethod();

                return method == null
                    ? null
                    : GetCliCommand(method);
            }
        }
    }
}
