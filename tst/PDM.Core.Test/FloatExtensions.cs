namespace PDM.Core.Test;

[ExcludeFromCodeCoverage]
public static class FloatExtensions
{
    public static float GetRandom(this float maxValue)
    {
        return maxValue.GetRandom(float.MinValue);
    }

    public static float GetRandom(this float maxValue, float minValue)
    {
        if (minValue > maxValue)
            throw new ArgumentOutOfRangeException(nameof(minValue), $"minValue {minValue} must be less than maxValue {maxValue}");

        var decimalPlaces = 5.GetRandom(3);
        var magnitude = Math.Pow(10, decimalPlaces);
        var maxMagnitude = Convert.ToSingle(maxValue * magnitude);
        var minMagnitude = Convert.ToSingle(minValue * magnitude);

        var maxInt = Convert.ToInt64(Math.Min(maxMagnitude, Convert.ToSingle(Int32.MaxValue)));
        var minInt = Convert.ToInt64(Math.Max(minMagnitude, Convert.ToSingle(Int32.MinValue)));

        return Convert.ToSingle(maxInt.GetRandom(minInt)) / Convert.ToSingle(magnitude);
    }
}
