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
        private static readonly Lazy<IReadOnlyList<CliCommandDefinition>> _commands =
            new Lazy<IReadOnlyList<CliCommandDefinition>>(LoadCliCommandsInternal);
        private static IReadOnlyList<CliCommandDefinition>
            LoadCliCommandsInternal()
        {
            var assembly = Assembly.GetEntryAssembly();

            if (assembly == null)
                return Array.Empty<CliCommandDefinition>();

            return GetCommands(assembly)
                .Select(command =>
                    command != null ?
                    CliCommandDefinition.CommandItem(
                        command.Command,
                        command.Description,
                        command.Parameters,
                        command.Options,
                        command.Method,
                        command.IsHelp) : null)
                .ToList();
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
        private static ApplicationCommand CreateCommand(
            MethodInfo method)
        {
            var defaulAttribute =
                 method.GetCustomAttribute<DefaultCommandAttribute>();

            if (defaulAttribute != null)
            {
                var instance = Activator.CreateInstance(method.DeclaringType);

                CLI.CLIApp.SetDefaultCommand(new DefaultCommand(method, (CliCommand)instance));
                return default;
            }

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

            return new ApplicationCommand(
                attribute.Command,
                attribute.ShortCommand,
                attribute.Description,
                method,
                parameters,
                options,
                attribute.IsHelp);
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
            => _commands.Value;
        public static IOrderedEnumerable<ApplicationCommand> GetCommands(
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
                    $"No concrete class inheriting from {nameof(CliCommand)} was found.");
            }

            if (commandTypes.Count > 1)
            {
                var types = string.Join(
                    ", ",
                    commandTypes.Select(x => x.FullName));

                throw new InvalidOperationException(
                    $"Only one class can inherit from {nameof(CliCommand)}. " +
                    $"Found {commandTypes.Count}: {types}.");
            }

            var commandType = commandTypes[0];

            var commandMethods = GetCommandMethods(commandType).ToList();

            if (commandMethods.Count == 0)
            {
                throw new InvalidOperationException(
                    $"Class '{commandType.FullName}' must contain at least one " +
                    $"{nameof(ApplicationCommandAttribute)} or " +
                    $"{nameof(DefaultCommandAttribute)}.");
            }

            return commandMethods
                .Select(CreateCommand)
                .Where(command => command != null)
                .OrderBy(command => command.Command);
        }      
        public static CliCommandDefinition GetCliCommand(
            MethodBase method)
        {
            if (method == null)
                throw new ArgumentNullException(nameof(method));

            return LoadCliCommands
                .FirstOrDefault(x => x.Method == method);
        }
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
