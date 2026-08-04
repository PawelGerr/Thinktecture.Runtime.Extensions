namespace Thinktecture.Runtime.Tests.TestAdHocUnions;

// ReSharper disable once InconsistentNaming
// Composes both features: the raw-value property is renamed to RawValue and made private. This only
// compiles if the generated member is named RawValue AND remains accessible from inside the type
// under a non-public access modifier.
[Union<int, string>(ValueMemberName = "RawValue",
                    ValueMemberAccessModifier = AccessModifier.Private,
                    SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
                    MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads)]
public partial class TestUnion_class_int_string_ValueMemberName_RawValue_ValueMemberAccessModifier_private
{
   public object? ReadRawValueInternally() => RawValue;
}
