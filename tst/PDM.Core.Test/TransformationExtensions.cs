namespace PDM.Core.Test;

public static class TransformationExtensions
{
    public static string Serialize(this IEnumerable<Entities.Transformation> transforms)
    {
        return System.Text.Json.JsonSerializer.Serialize(transforms);
    }
}
