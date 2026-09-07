using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MC.Code.CLI.Command
{
    [AttributeUsage(
    AttributeTargets.Method,
    AllowMultiple = false)]
    public sealed class DefaultCommandAttribute
                : Attribute
    {
    }
}
