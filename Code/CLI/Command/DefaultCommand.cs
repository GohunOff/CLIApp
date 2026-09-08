using MC.Code.CLI.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MC.Code.CLI.Command
{
    public sealed class DefaultCommand
    {
        public MethodInfo Method { get; }

        public CliCommand Instance { get; }
        /// <summary>
        /// Gets the type that declares the command method.
        /// </summary>
        public Type DeclaringType =>
            Method.DeclaringType;

        public DefaultCommand(MethodInfo method, CliCommand instance)
        {
            Method = method;
            Instance = instance;
        }
    }
}
