using Serilog;
using Xunit.Abstractions;
using PDM.Exceptions;

namespace PDM.Core.Test;

[ExcludeFromCodeCoverage]
public class WireTypeMismatchException_Throw_Should
{
    public WireTypeMismatchException_Throw_Should(ITestOutputHelper output)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Xunit(output)
            .MinimumLevel.Verbose()
            .CreateLogger();
    }

    [Fact]
    public void UseSensibleDefaultsIfTheDefaultConstructorIsUsed()
    {
        WireTypeMismatchException actual;
        try
        {
            throw new WireTypeMismatchException();
        }
        catch (WireTypeMismatchException ex)
        {
            actual = ex;
        }

        Assert.Equal((Enums.WireType)0, actual.ExpectedWireType);
        Assert.Equal(0, actual.FieldNumber);
        Assert.Null(actual.SuppliedValue);
    }

    // TODO: Reimplement tests

    //[Fact]
    //public void UseSensibleDefaultsIfNoTargetMappingIsSupplied()
    //{
    //    object value = string.Empty.GetRandom();

    //    WireTypeMismatchException actual;
    //    try
    //    {
    //        throw new WireTypeMismatchException(null!, value);
    //    }
    //    catch (WireTypeMismatchException ex)
    //    {
    //        actual = ex;
    //    }

    //    Assert.Equal((Enums.WireType)0, actual.ExpectedWireType);
    //    Assert.Equal(0, actual.FieldNumber);
    //}

    //[Fact]
    //public void UseASensibleDefaultIfNoTargetFieldIsSupplied()
    //{
    //    object value = string.Empty.GetRandom();
    //    var targetMapping = new Entities.Mapping(null!, string.Empty.GetRandom());

    //    WireTypeMismatchException actual;
    //    try
    //    {
    //        throw new WireTypeMismatchException(targetMapping, value);
    //    }
    //    catch (WireTypeMismatchException ex)
    //    {
    //        actual = ex;
    //    }

    //    Assert.Equal((Enums.WireType)0, actual.ExpectedWireType);
    //    Assert.Equal(0, actual.FieldNumber);
    //}

}
