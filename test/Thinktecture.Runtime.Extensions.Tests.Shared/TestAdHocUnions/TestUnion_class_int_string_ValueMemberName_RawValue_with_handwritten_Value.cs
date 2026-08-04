namespace Thinktecture.Runtime.Tests.TestAdHocUnions;

// ReSharper disable once InconsistentNaming
// The motivating use case: the generated raw-value property is renamed to RawValue, which frees the
// Value identifier so the user can hand-write their own Value property of a different type. This
// only compiles if the generated member is really renamed and does not collide with the
// hand-written Value.
[Union<int, string>(ValueMemberName = "RawValue")]
public partial class TestUnion_class_int_string_ValueMemberName_RawValue_with_handwritten_Value
{
   public string Value => RawValue?.ToString() ?? "";
}
