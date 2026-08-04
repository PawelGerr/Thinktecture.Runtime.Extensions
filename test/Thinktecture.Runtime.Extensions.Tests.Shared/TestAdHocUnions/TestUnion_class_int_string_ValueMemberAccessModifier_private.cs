namespace Thinktecture.Runtime.Tests.TestAdHocUnions;

// ReSharper disable once InconsistentNaming
[Union<int, string>(ValueMemberAccessModifier = AccessModifier.Private,
                    SwitchMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads,
                    MapMethods = SwitchMapMethodsGeneration.DefaultWithPartialOverloads)]
public partial class TestUnion_class_int_string_ValueMemberAccessModifier_private
{
   // Reads the now-private Value from inside the type. This only compiles if in-type access to the
   // generated Value property is still allowed under a non-public access modifier.
   public object? ReadValueInternally() => Value;
}
