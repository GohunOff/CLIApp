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
        private static void ValidateCommandMethod(
                 Type declaringType,
                 MethodInfo method)
        {
            if (!typeof(CliCommand).IsAssignableFrom(declaringType))
            {
                throw new InvalidOperationException(
                    $"'{method.DeclaringType?.FullName}.{method.Name}' " +
                    $"jest oznaczona jako ApplicationCommand, ale jej klasa " +
                    $"nie dziedziczy po CliCommand.");
            }
        }

        private const BindingFlags CommandBindingFlags =
        BindingFlags.Public |
        BindingFlags.NonPublic |
        BindingFlags.Instance |
        BindingFlags.Static;

        public static IOrderedEnumerable<ApplicationCommand> GetCommands(
            Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));



            return assembly
                .GetTypes()
                .Where(IsCommandType)
                .SelectMany(GetCommandMethods)
                .Select(CreateCommand)
                .OrderBy(command => command.Command);
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

                ValidateCommandMethod(type, method);

                yield return method;
            }
        }


        private static bool HasCommandAttribute(MethodInfo method)
        {
            return method.IsDefined(
                typeof(ApplicationCommandAttribute),
                inherit: false);
        }

        private static ApplicationCommand CreateCommand(
      MethodInfo method)
        {
            var attribute =
                method.GetCustomAttribute<ApplicationCommandAttribute>();

            if (attribute == null)
            {
                throw new InvalidOperationException(
                    $"Metoda '{method.DeclaringType?.FullName}.{method.Name}' " +
                    $"nie posiada ApplicationCommandAttribute.");
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

        public static IReadOnlyList<CliCommandDefinition> LoadCliCommands()
        {
            return _commands.Value;
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
                    CliCommandDefinition.CommandItem(
                        command.Command,
                        command.Description,
                        command.Parameters,
                        command.Options,
                        command.Method,
                        command.IsHelp))
                .ToList();
        }

        public static CliCommandDefinition GetCliCommand(
                  MethodBase method)
        {
            if (method == null)
                throw new ArgumentNullException(nameof(method));

            return LoadCliCommands()
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
