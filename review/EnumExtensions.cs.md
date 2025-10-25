# Issues found in src/Thinktecture.Runtime.Extensions.SourceGenerator/Extensions/EnumExtensions.cs

## Errors

1) Invalid array cast for underlying values
- Code: `public static readonly int[] Values = (int[])Enum.GetValues(typeof(T));`
- Problem: `Enum.GetValues(typeof(T))` returns an array of the enum type (`T[]`), not an array of the underlying numeric type. Casting `T[]` to `int[]` always fails at runtime with InvalidCastException, regardless of the enum’s underlying type.
- Impact: Any first access to `EnumHelper<T>` (e.g., calling `GetValidValue`) will throw.
- Fix options:
  - Use `Enum.GetValuesAsUnderlyingType(typeof(T))` (returns an `Array` of the actual underlying numeric type), and adjust comparison logic accordingly.
  - If the API is intended to support only int-based enums, guard explicitly:
    ```
    if (Enum.GetUnderlyingType(typeof(T)) != typeof(int)) throw new NotSupportedException(...);
    ```
    and remove the invalid cast.

2) `GetValidValue<T>(this int value)` incompatible with non-int underlying enums
- Code: `public static T? GetValidValue<T>(this int value) where T : struct, Enum`
- Problems:
  - The parameter is `int`, but enums may use any integral underlying type (`sbyte, byte, short, ushort, int, uint, long, ulong`). For non-int underlying enums this can truncate or mis-compare values.
  - Combined with the broken `Values` array cast, the method is currently non-functional.
- Impact: Incorrect behavior or exceptions for enums not using `int` as underlying type.
- Fix options:
  - Generalize input type by comparing via `Convert.ToUInt64(value)` against enum values converted to `UInt64`, or
  - Use `Enum.GetValuesAsUnderlyingType` and compare using typed values, or
  - Restrict to int-based enums with a runtime check and document the limitation.

## Warnings

1) `IsValid<T>` does not account for [Flags] combinations
- Code: `Array.IndexOf(EnumHelper<T>.Items, item) >= 0`
- Problem: Returns `false` for valid bitwise combinations unless explicitly declared as named members.
- Risk: Surprising semantics for flags enums; may be intended, but if general validity for flags is desired, this is too strict.
- Suggestion: If flags support is needed, validate via bitwise decomposition against defined values or document current behavior clearly.

2) Redundant generic constraints
- Code: `where T : struct, Enum` (in `GetValidValue`)
- Problem: `Enum` constraint implies value type; adding `struct` is redundant.
- Suggestion: Simplify to `where T : Enum` for consistency with `IsValid<T>`.
