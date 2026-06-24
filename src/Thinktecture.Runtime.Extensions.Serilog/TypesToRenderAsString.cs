namespace Thinktecture;

/// <summary>
/// Selects which Thinktecture type families are rendered as a flat string (via <c>ToString()</c>)
/// instead of being unwrapped to their underlying payload when destructured by Serilog.
/// </summary>
[Flags]
public enum TypesToRenderAsString
{
	/// <summary>No family is rendered as a string; all supported families are unwrapped (the default).</summary>
	None = 0,

	/// <summary>Keyed Smart Enums are rendered as <c>ToString()</c> instead of their key.</summary>
	SmartEnums = 1,

	/// <summary>Keyed (simple) Value Objects are rendered as <c>ToString()</c> instead of their key.</summary>
	ValueObjects = 2,

	/// <summary>Ad-hoc Unions are rendered as <c>ToString()</c> instead of their value.</summary>
	AdHocUnions = 4,

	/// <summary>All supported families are rendered as <c>ToString()</c>.</summary>
	All = SmartEnums | ValueObjects | AdHocUnions
}
