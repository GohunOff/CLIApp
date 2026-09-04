using MC.Code.CLI.ARGS;
using MC.Code.CLI.Base;
using MC.Code.CLI.Formatter;
using MC.Code.CLI.RES;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;

namespace MC.Code.CLI.Command
{
    public sealed class ApplicationCommandRunner
    {
        private static readonly LocalizedResource _messages
                    = new LocalizedResource(new ResourceManager
              ("MC.Code.CLI.Command.Resources.ApplicationCommandRunner",
              typeof(ApplicationCommandRunner).Assembly));
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
                Console.WriteLine(_messages.Get("UnknownCommand", args.Name));
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
              
                Console.WriteLine(_messages.Get("ParameterError", exception.Message));

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

                Console.WriteLine(_messages.Get("CommandExecutionError", cause.Message));

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
                        throw new ArgumentException(_messages.Get("UnknownOption", argument));
                    }

                    if (values2.ContainsKey(property))
                    {
                        throw new ArgumentException(_messages.Get("OptionSpecifiedMoreThanOnce", argument));
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
                        throw new ArgumentException(_messages.Get("UnknownProperty", argument));
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
                        throw new Exception(_messages.Get("ConversionError", (resultIndex + 1), parameter.Name, parameter.ParameterType));
                    }
                    continue;
                }

                if (parameter.IsRequired)
                {
                    throw new ArgumentException(_messages.Get("MissingRequiredParameter", parameter.Name));
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
                        _messages.Get("OptionsCannotBeUsedSimultaneously", string.Join(", ", selected.Select(x => x.Name)))
                        );
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
                              _messages.Get("OptionInvalidType", option.Name, option.ParameterType));
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
                           _messages.Get("OptionBooleanRequired", option.Name, option.ParameterType));
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
                        _messages.Get("ValueNotValidEnum", value, type.Name));
                }
            }

            // Guid
            if (type == typeof(Guid))
            {
                Guid guid;

                if (Guid.TryParse(value, out guid))
                    return guid;

                throw new FormatException(
                        _messages.Get("ValueNotValidGuid", value));
            }

            // char
            if (type == typeof(char))
            {
                if (value.Length == 1)
                    return value[0];
         
                throw new FormatException(
                      _messages.Get("ValueNotSingleCharacter", value));
            }

            // bool
            if (type == typeof(bool))
            {
                bool boolean;

                if (bool.TryParse(value, out boolean))
                    return boolean;
 
                throw new FormatException(
                   _messages.Get("ValueNotValidBoolean", value));
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
                         _messages.Get("ValueCannotConvert", value, type.Name),
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
                            _messages.Get("ValueCannotConvert", value, type.Name),
                        ex);
                }
            }
       
            throw new NotSupportedException(
                _messages.Get("ConversionNotSupported", type.FullName));
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
                         _messages.Get("ParameterTypeMismatch", methodParameter.Name, parameter.Index, methodParameter.ParameterType));
                }

                if (properties.Count(x => x.Index == parameter.Index) > 1)
                {

                    throw new InvalidOperationException(
                        _messages.Get("ParameterDefinedMoreThanOnce", parameter.Name));
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
                        _messages.Get("OptionBooleanRequired", methodParameter.Name, option.Index, methodParameter.ParameterType, option.Name, option.ParameterType));
                }

                
                     
                // Option nie może współistnieć z Parameter
                if (properties.Any(x => x.Index == option.Index))
                {   
                    throw new InvalidOperationException(
                    _messages.Get("ParameterAndOptionConflict", methodParameter.Name, option.Index));
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
                             _messages.Get("OptionsDifferentTypes", option.Index, parameterType, option.ParameterType));
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
                       _messages.Get("OptionDefinedMoreThanOnce", option.Name));
                }

                if (!string.IsNullOrWhiteSpace(option.ShortName) &&
                    !names.Add(option.ShortName))
                {

                    throw new InvalidOperationException(
                      _messages.Get("OptionAliasDefinedMoreThanOnce", option.ShortName));
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
                        _messages.Get("ParameterNoRole", parameter.Name, index));
                }

                if (isParameter && isOption)
                {
                    throw new InvalidOperationException(
                        _messages.Get("ParameterNoRole", parameter.Name, index));
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
                   _messages.Get("InvalidParameterIndex", name, index));
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
