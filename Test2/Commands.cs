using MC.Code.CLI.Base;
using MC.Code.CLI.Command;
using System;

namespace Test
{
    public class Commands : CliCommand
    {
        [ApplicationCommand(
            "help",
            "Pokazuje pomoc",
            isHelp:true)]
        public void Help()
        {
        }

        [ApplicationCommand(
            "version",
            "Pokazuje wersję")]
        public void Version()
        {
            Console.WriteLine("sersja test");
        }

        [ApplicationCommand(
            "config",
            "Konfiguracja aplikacji")]
        public void Config()
        {
            Console.WriteLine("start");
        }

        [ApplicationCommand(
            "start",
            "ueucmaia apikaje")]
        public void Start(
             [ApplicationParameter(
                description: "automat")]
            string par,
                    [ApplicationParameter(
                description: "automat2")]
             string par2 = "oo",
           [ApplicationOption(
                "slow",
                "Uruchamia tryb szybki")]
            [ApplicationOption(
                "fast",
                "Uruchamia tryb szybki")]
             ttE fast = ttE.slow,
                 [ApplicationOption(
                "gruby",
                "inna opcja")]
            bool gruby = false
            )
        {
            var ss = this.CurrentCommand;
            Console.WriteLine($"start sss {par} oo {par2}  {fast}");
        }
    }
}
