namespace Thinktecture.Runtime.Tests.TestAdHocUnions;

// ReSharper disable once InconsistentNaming
[Union<int, string>(ValueMemberName = "RawValue",
                    SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
                    MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads)]
public partial class TestUnion_class_int_string_ValueMemberName_RawValue
{
   // Reads the renamed raw-value property. This only compiles if the generated member is named
   // RawValue instead of Value.
   public object? ReadRawValueInternally() => RawValue;
}
