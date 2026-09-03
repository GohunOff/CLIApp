using MC.Code.CLI;
using MC.Code.CLI.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var output = new ConsoleApplicationInfoOutput();
            CLIApp.Init(
                "My application",
                output,
                CLIApp.NoArgsBehavior.ShowHelp,
                args);

            if (!CLIApp.TryShowHelp())
            {
                CLIApp.Run();
            }
        }
    }
}
