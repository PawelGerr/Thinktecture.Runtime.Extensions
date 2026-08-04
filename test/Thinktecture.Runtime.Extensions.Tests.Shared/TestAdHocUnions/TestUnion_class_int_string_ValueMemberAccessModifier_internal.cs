namespace Thinktecture.Runtime.Tests.TestAdHocUnions;

// ReSharper disable once InconsistentNaming
[Union<int, string>(ValueMemberAccessModifier = AccessModifier.Internal,
                    SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
                    MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads)]
public partial class TestUnion_class_int_string_ValueMemberAccessModifier_internal
{
   // Reads the now-internal Value from inside the type to prove in-type access still compiles.
   public object? ReadValueInternally() => Value;
}
