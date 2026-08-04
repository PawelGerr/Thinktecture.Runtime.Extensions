namespace Thinktecture.Runtime.Tests.TestAdHocUnions;

// ReSharper disable once InconsistentNaming
[Union<string, int>(ValueMemberName = "RawValue",
                    SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
                    MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads)]
public partial struct TestUnion_struct_string_int_ValueMemberName_RawValue
{
   // Reads the renamed raw-value property on a struct union (exercises the index-check value getter).
   public object? ReadRawValueInternally() => RawValue;
}
