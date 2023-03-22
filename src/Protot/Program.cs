using Protot.Core;
using Protot.Entities;
using Protot.Extensions;


PrototOptions prototOptions = new PrototOptions();
ReadArgument(args, prototOptions);

if (prototOptions.PrintHelp)
{
    Console.Write(ArgumentExtensions.PrintHelp());
}
else
{
    if (prototOptions.Validate())
    {
        try
        {
            var source = await prototOptions.SourcePath!.ReadFileText();
            var target = await prototOptions.TargetPath!.ReadFileText();
            var transforamtionConfig = await prototOptions.TransformationConfigPath!.ParseProtoTransformation();
            var prototMapper = new PrototMapper(source, target, transforamtionConfig);
            var outTransformations = await prototMapper.CompileAsync();
            var isSaved = prototOptions.TransformationConfigOutPath != null &&
                          await outTransformations.WriteTransformation(prototOptions.TransformationConfigOutPath);
            Console.WriteLine(!isSaved
                ? "Unable to Save Transformation"
                : $"Successfully Save Transformation to {prototOptions.TransformationConfigOutPath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error {ex.Message}");
        }
    }
}

static void ReadArgument(string[] strings, PrototOptions options)
{
    foreach (var arg in strings)
    {
        if (arg.StartArgsWith("--source_path"))
        {
            options.SourcePath = arg.GetArgumentValue();
        }
        if (arg.StartArgsWith("--target_path"))
        {
            options.TargetPath = arg.GetArgumentValue();
        }
        if (arg.StartArgsWith("--transformation_config_path"))
        {
            options.TransformationConfigPath = arg.GetArgumentValue();
        }

        if (arg.StartArgsWith("--transformation_out_path"))
        {
            options.TransformationConfigOutPath = arg.GetArgumentValue();
        }

        if (arg.StartArgsWith("--help") || arg.StartArgsWith("--h"))
        {
            options.PrintHelp = true;
        }
    }
}

