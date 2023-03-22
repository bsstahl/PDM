using System.Text;

namespace Protot.Extensions;

internal static class ArgumentExtensions
{
    internal static string? GetArgumentValue(this string argument)
    {
        var values = argument.Split("=", StringSplitOptions.RemoveEmptyEntries);
        return values.Length == 1 ? null : values[1];
    }

    internal static string PrintHelp()
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("--source_path                     Source Proto file Path");
        stringBuilder.AppendLine("--target_path                     Target Proto file Path");
        stringBuilder.AppendLine("--transformation_config_path      Transformation configuration path");
        stringBuilder.AppendLine("--transformation_out_path         Transformation configuration output path");


        return stringBuilder.ToString();
    }
}