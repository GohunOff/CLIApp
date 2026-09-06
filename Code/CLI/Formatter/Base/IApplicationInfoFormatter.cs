using MC.Code.CLI.Application;

namespace MC.Code.CLI.Formatter.Base
{
    public interface IApplicationInfoFormatter
    {
        string Format(ApplicationInfo info);

       // ApplicationInfoLayout Layout { get; }
    }
}