using PDM.Entities;

namespace PDM.Exceptions;

public class WireTypeMismatchException: Exception
{
    const string _unknown = "unknown";

    public Enums.WireType ExpectedWireType { get; set; }
    public int FieldNumber { get; set; }
    public bool IsSourceField { get; set; }
    public object SuppliedValue { get; set; }

    public WireTypeMismatchException()
        : this("A mismatch has occurred in WireType specifications")
    { }

    public WireTypeMismatchException(Mapping targetMapping, dynamic value)
        : this($"Expected WireType {targetMapping?.TargetField?.WireType.ToString() ?? _unknown } but received a value of {value} for target field {targetMapping?.TargetField?.Key.ToString() ?? "unknown"}")
    {
        this.ExpectedWireType = targetMapping?.TargetField?.WireType ?? Enums.WireType.VarInt;
        this.FieldNumber = targetMapping?.TargetField?.Key ?? 0;
        this.IsSourceField = false;
        this.SuppliedValue = value;
    }

    public WireTypeMismatchException(string message) 
        : base(message)
    { }

    public WireTypeMismatchException(string message, Exception innerException) 
        : base(message, innerException)
    { }

}
