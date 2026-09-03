using MC.Code.CLI.ARGS;
using MC.Code.CLI.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace MC.Code.CLI.Command
{
    public sealed class ApplicationCommandRunner
    {
        public bool Execute(ParsedCommand args)
        {
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            if (string.IsNullOrEmpty(args.Name))
                return false;

            var command = CliCommand
                .LoadCliCommands()
                .FirstOrDefault(x =>
                    x.Command.Equals(
                        args.Name,
                        StringComparison.OrdinalIgnoreCase));

            if (command == null)
            {
                Console.WriteLine($"Unknown command: {args.Name}");
                return false;
            }

            object[] parametr;

            try
            {
                parametr =
                    ParseCommands(
                        command.Method,
                        command.Properties,
                        command.Options,
                        args);
            }
            catch (Exception exception)
            {
                Console.WriteLine(
                    $"Parameter error: {exception.Message}");

                return false;
            }

            try
            {
                object instance = null;

                if (!command.Method.IsStatic)
                {
                    instance =
                        Activator.CreateInstance(
                            command.Method.DeclaringType);
                }

                command.Method.Invoke(
                    instance,
                    parametr);

                return true;
            }
            catch (TargetInvocationException exception)
            {
                Exception cause =
                    exception.InnerException ?? exception;

                Console.WriteLine(
                    $"Command execution error: {cause.Message}");

                return false;
            }
        }

        private object[] ParseCommands(
            MethodInfo method,
            IReadOnlyList<ApplicationParameterAttribute> properties,
            IReadOnlyList<ApplicationOptionAttribute> options,
            ParsedCommand args)
        {
            ParameterInfo[] methodParameters =
                    method.GetParameters();

            ValidateCommandDefinition(
                method,
                methodParameters,
                properties,
                options);

            var values =
                new Dictionary<
                    ApplicationParameterAttribute,
                    string>();

            var values2 =
            new Dictionary<ApplicationOptionAttribute, string>();

            for (int i = 0; i < args.Count; i++)
            {
                if (args[i].IsOption)
                {
                    string argument = args[i].Name;
                    ApplicationOptionAttribute property =
                           FindOption(
                               options, args[i].Name);
                    if (property == null)
                    {
                        throw new ArgumentException(
                            $"Unknown option: {argument}");
                    }

                    if (values2.ContainsKey(property))
                    {
                        throw new ArgumentException(
                            $"The option '{argument}' was specified more than once.");
                    }

                    values2[property] = argument;
                }
                else
                {
                    string argument = args[i].Value;
                    ApplicationParameterAttribute property =
                           FindProperty(
                               properties, args[i].ArgumentIndex);

                    if (property == null)
                    {
                        throw new ArgumentException(
                            $"Unknown property: {argument}");
                    }
                    values[property] = argument;
                }
            }

            var result =
                new object[methodParameters.Length];

            foreach (var parameter in properties)
            {
                int resultIndex = parameter.Index - 1;

                if (values.TryGetValue(
                    parameter,
                    out string value))
                {
                    try
                    {
                        result[resultIndex] =
                            ConvertValue(
                                value,
                                parameter.ParameterType);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(
                            $"{ex.Message}{Environment.NewLine}" +
                            $"expected parameter %{resultIndex + 1} " +
                            $"({parameter.Name}) type {parameter.ParameterType}");
                    }
                    continue;
                }

                if (parameter.IsRequired)
                {
                    throw new ArgumentException(
                        $"Missing required parameter: {parameter.Name}");
                }

                result[resultIndex] = parameter.HasDefaultValue
                           ? parameter.DefaultValue
                           : GetDefaultValue(parameter.ParameterType);
            }

            foreach (var group in options.GroupBy(x => x.Index))
            {
                var selected =
                    group.Where(x => values2.ContainsKey(x))
                         .ToList();

                if (selected.Count > 1)
                {
                    throw new ArgumentException(
                        $"The following options cannot be used simultaneously: " +
                        $"{string.Join(", ", selected.Select(x => x.Name))}. " +
                        $"The options apply to the same parameter.");
                }



                if (selected.Count == 0)
                {
                    var option2 = group.First();
                    var resultIndex2 = option2.Index - 1;

                    result[resultIndex2] =
                        option2.HasDefaultValue
                            ? option2.DefaultValue
                            : GetDefaultValue(option2.ParameterType);

                    continue;
                }

                var option = selected[0];
                var resultIndex = option.Index - 1;

                if (group.Count() > 1)
                {
                    if (!option.ParameterType.IsEnum)
                    {
                        throw new ArgumentException(
                            $"Option {{option.Name}} has an invalid type. " +
                            $"{option.ParameterType}. " +
                            $"For multiple definitions of ApplicationOptionAttribute " +
                            $"An enum is required.");
                    }

                    var value = values2[option];
                    result[resultIndex] =
                        ConvertValue(
                            value,
                            option.ParameterType);
                }
                else
                {
                    if (option.ParameterType != typeof(bool))
                    {
                        throw new ArgumentException(
                            $"Option {option.Name} has an invalid type. " +
                            $"{option.ParameterType}. " +
                            $"Allowed type: System.Boolean.");
                    }

                    result[resultIndex] = true;
                }
            }


            return result;
        }

        private ApplicationParameterAttribute FindProperty(
            IReadOnlyList<ApplicationParameterAttribute> properties,
            int index)
        {
            return properties.FirstOrDefault(x => x.Index == index);
        }

        private ApplicationOptionAttribute FindOption(
            IReadOnlyList<ApplicationOptionAttribute> options,
            string name)
        {
            return options.FirstOrDefault(x =>
            string.Equals(
                x.Name,
                name,
                StringComparison.OrdinalIgnoreCase) ||
            string.Equals(
                x.ShortName,
                name,
                StringComparison.OrdinalIgnoreCase));
        }

        private object ConvertValue(
            string value,
            Type type)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (type == null)
                throw new ArgumentNullException(nameof(type));

            // Nullable<T>
            Type nullableType =
                Nullable.GetUnderlyingType(type);

            if (nullableType != null)
            {
                if (string.IsNullOrWhiteSpace(value))
                    return null;

                return ConvertValue(
                    value,
                    nullableType);
            }

            // string
            if (type == typeof(string))
                return value;

            // enum
            if (type.IsEnum)
            {
                try
                {
                    return Enum.Parse(
                        type,
                        value,
                        true);
                }
                catch (ArgumentException)
                {
                    throw new FormatException(
                        string.Format(
                            "The value '{0}' is not a valid value of type {1}.",
                            value,
                            type.Name));
                }
            }

            // Guid
            if (type == typeof(Guid))
            {
                Guid guid;

                if (Guid.TryParse(value, out guid))
                    return guid;

                throw new FormatException(
                    string.Format(
                        "The value '{0}' is not a valid GUID..",
                        value));
            }

            // char
            if (type == typeof(char))
            {
                if (value.Length == 1)
                    return value[0];

                throw new FormatException(
                    string.Format(
                        "The value '{0}' is not a single character.",
                        value));
            }

            // bool
            if (type == typeof(bool))
            {
                bool boolean;

                if (bool.TryParse(value, out boolean))
                    return boolean;

                throw new FormatException(
                    string.Format(
                        "The value '{0}' is not a valid boolean value.",
                        value));
            }

            // Wszystkie typy IConvertible:
            // byte, short, int, long, float, double,
            // decimal, uint, ushort, ulong, sbyte...
            if (typeof(IConvertible).IsAssignableFrom(type))
            {
                try
                {
                    return Convert.ChangeType(
                        value,
                        type,
                        CultureInfo.InvariantCulture);
                }
                catch (Exception ex)
                    when (ex is FormatException ||
                          ex is InvalidCastException ||
                          ex is OverflowException)
                {
                    throw new FormatException(
                        string.Format(
                            "The value '{0}' cannot be converted to type {1}.",
                            value,
                            type.Name),
                        ex);
                }
            }

            // Typy posiadające TypeConverter
            TypeConverter converter =
                TypeDescriptor.GetConverter(type);

            if (converter.CanConvertFrom(typeof(string)))
            {
                try
                {
                    return converter.ConvertFromInvariantString(value);
                }
                catch (Exception ex)
                    when (ex is FormatException ||
                          ex is InvalidCastException)
                {
                    throw new FormatException(
                        string.Format(
                            "The value '{0}' cannot be converted to type {1}.",
                            value,
                            type.Name),
                        ex);
                }
            }

            throw new NotSupportedException(
                string.Format(
                    "Conversion of the CLI value to type {0} is not supported.",
                    type.FullName));
        }

        private void ValidateCommandDefinition(
                MethodInfo method,
                  ParameterInfo[] parameters,
                IReadOnlyList<ApplicationParameterAttribute> properties,
                IReadOnlyList<ApplicationOptionAttribute> options)
        {
            if (method == null)
                throw new ArgumentNullException(nameof(method));

            if (properties == null)
                throw new ArgumentNullException(nameof(properties));

            if (options == null)
                throw new ArgumentNullException(nameof(options));

            // ---------------------------------
            // ApplicationParameter
            // ---------------------------------

            foreach (var parameter in properties)
            {
                ValidateParameterIndex(
                    parameter.Index,
                    parameter.Name,
                    parameters.Length);

                ParameterInfo methodParameter = parameters[parameter.Index - 1];

                if (methodParameter.ParameterType != parameter.ParameterType)
                {
                    throw new InvalidOperationException(
                        $"Parameter '{methodParameter.Name}' ({parameter.Index}) " +
                        $"He's got a type. {methodParameter.ParameterType}, " +
                        $"but ApplicationParameter declares the type " +
                        $"{parameter.ParameterType}.");
                }

                if (properties.Count(x => x.Index == parameter.Index) > 1)
                {
                    throw new InvalidOperationException(
                        $"Parameter '{parameter.Name}' remained " +
                        $"defined as ApplicationParameter more than once.");
                }
            }

            // ---------------------------------
            // ApplicationOption
            // ---------------------------------

            foreach (var option in options)
            {
                ValidateParameterIndex(
                    option.Index,
                    option.Name,
                    parameters.Length);

                var methodParameter =
                    parameters[option.Index - 1];

                if (methodParameter.ParameterType != option.ParameterType)
                {
                    throw new InvalidOperationException(
                        $"Parameter '{methodParameter.Name}' ({option.Index}) " +
                        $"He's got a type. {methodParameter.ParameterType}, " +
                        $"but ApplicationOption'{option.Name}' declares the type" +
                        $"{option.ParameterType}.");
                }


                // Option nie może współistnieć z Parameter
                if (properties.Any(x => x.Index == option.Index))
                {   
                    throw new InvalidOperationException(
                        $"Parameter '{methodParameter.Name}' ({option.Index}) " +
                        $"was simultaneously defined as " +
                        $"ApplicationParameter and ApplicationOption.");
                }
            }

            // ---------------------------------
            // Opcje należące do tego samego
            // parametru muszą mieć ten sam typ
            // ---------------------------------

            foreach (var group in options.GroupBy(x => x.Index))
            {
                Type parameterType =
                    group.First().ParameterType;

                foreach (var option in group)
                {
                    if (option.ParameterType != parameterType)
                    {
                        throw new InvalidOperationException(
                            $"Options belonging to the parameter " +
                            $"{option.Index} \they have different types:" +
                            $"{parameterType} and {option.ParameterType}.");
                    }
                }
            }

            // ---------------------------------
            // Nazwy opcji muszą być unikalne
            // ---------------------------------

            var names =
                new HashSet<string>(
                    StringComparer.OrdinalIgnoreCase);

            foreach (var option in options)
            {
                if (!names.Add(option.Name))
                {
                    throw new InvalidOperationException(
                        $"Option '{option.Name}' has been defined more than once.");
                }

                if (!string.IsNullOrWhiteSpace(option.ShortName) &&
                    !names.Add(option.ShortName))
                {
                    throw new InvalidOperationException(
                        $"Option name/alias '{option.ShortName}' " +
                        $"has been defined more than once.");
                }
            }

            // ---------------------------------
            // Każdy parametr metody musi mieć
            // dokładnie jedną rolę
            // ---------------------------------

            foreach (var parameter in parameters)
            {
                int index = parameter.Position + 1;

                bool isParameter =
                    properties.Any(x => x.Index == index);

                bool isOption =
                    options.Any(x => x.Index == index);

                if (!isParameter && !isOption)
                {
                    throw new InvalidOperationException(
                        $"Parameter '{parameter.Name}' ({index}) " +
                        $"There is no defined role.");
                }

                if (isParameter && isOption)
                {
                    throw new InvalidOperationException(
                        $"Parameter '{parameter.Name}' ({index}) " +
                        $"has more than one role.");
                }
            }
        }

        private void ValidateParameterIndex(
            int index,
            string name,
            int parameterCount)
        {

            if (index <= 0 || index > parameterCount)
            {
                throw new InvalidOperationException(
                    $"Element '{name}' has an invalid index {index}.");
            }
        }



        private object GetDefaultValue(
          Type type)
        {
            if (type.IsValueType)
                return Activator.CreateInstance(type);

            return null;
        }
    }
}
