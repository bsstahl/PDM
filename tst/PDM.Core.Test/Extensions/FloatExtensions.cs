using PDM.Core.Test.Extensions;

namespace PDM.Core.Test.Extensions;

[ExcludeFromCodeCoverage]
public static class FloatExtensions
{
    public static float GetRandom(this float maxValue)
        => maxValue.GetRandom(float.MinValue);

    public static float GetRandom(this float maxValue, float minValue)
    {
        if (minValue > maxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(minValue), $"minValue {minValue} must be less than maxValue {maxValue}");
        }

        var decimalPlaces = 5.GetRandom(3);
        var magnitude = Math.Pow(10, decimalPlaces);
        var maxMagnitude = Convert.ToSingle(maxValue * magnitude);
        var minMagnitude = Convert.ToSingle(minValue * magnitude);

        var maxInt = Convert.ToInt64(Math.Min(maxMagnitude, Convert.ToSingle(int.MaxValue)));
        var minInt = Convert.ToInt64(Math.Max(minMagnitude, Convert.ToSingle(int.MinValue)));

        return Convert.ToSingle(maxInt.GetRandom(minInt)) / Convert.ToSingle(magnitude);
    }
}
