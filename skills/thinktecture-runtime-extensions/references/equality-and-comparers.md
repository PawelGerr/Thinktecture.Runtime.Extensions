# Equality & Comparers

How generated equality (`Equals`/`GetHashCode`/`==`/`!=`) and ordering (`IComparable`/`<` `<=` `>` `>=`)
pick the comparer for the underlying value(s). This is shared by **Smart Enums** (keyed) and **Value
Objects** (simple keys and complex members) — the attributes and the predefined accessors are the same.
Load it whenever a type has a `string` key/member, or you need case- or culture-specific comparison.

## The default: strings compare `OrdinalIgnoreCase`

For a `string` key/member the generator defaults to **`OrdinalIgnoreCase`**, *not* the BCL default
(ordinal, case-sensitive). The library treats case-sensitive string comparison as usually-a-bug, so it
is opt-in. Because this default is a deliberate choice you should make explicit, the analyzer warns:

- **TTRESG048** (Warning) — a string-keyed simple Value Object has no `[KeyMemberEqualityComparer<…>]`.
- **TTRESG049** (Warning) — a complex Value Object with string members sets no comparison.

Smart Enums use the same `OrdinalIgnoreCase` default for string keys; set a comparer when you need a
different rule (e.g. case-sensitive codes).

## The three attributes

| Attribute | Arity | Applies to | Controls |
|---|---|---|---|
| `[KeyMemberEqualityComparer<TAccessor, TKey>]` | 2 | Smart Enum / **simple** VO (on the type) | equality of the key (`Equals`/hash/`==`) |
| `[KeyMemberComparer<TAccessor, TKey>]` | 2 | Smart Enum / **simple** VO (on the type) | ordering of the key (`IComparable`/`<` `>`) |
| `[MemberEqualityComparer<TAccessor, TMember>]` | 2 | **complex** VO (on a property/field) | equality of that one member |

`TAccessor` is a type implementing `IEqualityComparerAccessor<T>` (and/or `IComparerAccessor<T>`) — use a
predefined `ComparerAccessors.*` (below) or your own. The accessor's `T` must match the member type
(mismatch is **TTRESG041**).

**Equality vs ordering are separate.** `KeyMemberEqualityComparer` drives `Equals`/hash/`==`;
`KeyMemberComparer` drives `IComparable`/`<`/`>`. Defining one but not the other is flagged for
consistency: **TTRESG102** (comparer without equality comparer) / **TTRESG103** (equality comparer
without comparer). Complex VOs have **no** key-level ordering comparer — equality is per-member only.

## Predefined `ComparerAccessors` (namespace `Thinktecture`)

Verified set (each supports both equality and ordering for `string`):

- `ComparerAccessors.StringOrdinal`
- `ComparerAccessors.StringOrdinalIgnoreCase`
- `ComparerAccessors.CurrentCulture`
- `ComparerAccessors.CurrentCultureIgnoreCase`
- `ComparerAccessors.InvariantCulture`
- `ComparerAccessors.InvariantCultureIgnoreCase`
- `ComparerAccessors.Default<T>` — `EqualityComparer<T>.Default` / `Comparer<T>.Default` for any `T`

## Snippets

```csharp
// Smart Enum — case-sensitive string codes (override the OrdinalIgnoreCase default)
[SmartEnum<string>]
[KeyMemberEqualityComparer<ComparerAccessors.StringOrdinal, string>]
[KeyMemberComparer<ComparerAccessors.StringOrdinal, string>]
public partial class ProductCategory
{
    public static readonly ProductCategory Fruits = new("fruits");   // distinct from "FRUITS"
}

// Simple Value Object — exact, case-sensitive equality
[ValueObject<string>]
[KeyMemberEqualityComparer<ComparerAccessors.StringOrdinal, string>]
public sealed partial class ProductName;

// Complex Value Object — per-member comparers + a global default
[ComplexValueObject(DefaultStringComparison = StringComparison.OrdinalIgnoreCase)]
public partial class Account
{
    [MemberEqualityComparer<ComparerAccessors.StringOrdinal, string>]
    public string Identifier { get; }     // case-sensitive

    public string DisplayName { get; }    // uses the DefaultStringComparison above

    [IgnoreMember]
    public DateTime LoadedAt { get; }     // excluded from equality and the factory
}
```

- **Complex VOs**: set `DefaultStringComparison` on `[ComplexValueObject]` for all string members at
  once, override individual ones with `[MemberEqualityComparer<…>]`, and exclude a member from generated
  equality/factory with `[IgnoreMember]`.
- For **ordering** of a simple VO, add `[KeyMemberComparer<…>]` alongside the equality comparer.

## Common pitfalls

- Leaving a string key/member on the default and getting surprised by case-insensitivity — be explicit
  (silenced by adding the comparer; **TTRESG048**/**049**).
- Setting only an equality comparer **or** only an ordering comparer (**TTRESG102**/**103**).
- Accessor generic argument not matching the member type (**TTRESG041**).

## Worked examples in this repo

- Smart Enum, case-sensitive: `samples/Basic.Samples/SmartEnums/ProductCategoryWithCaseSensitiveComparer.cs`.
- Complex VO, custom equality: `samples/Basic.Samples/ValueObjects/ComplexValueObjectWithCustomEqualityComparison.cs`.
- Per-member comparers: `test/Thinktecture.Runtime.Extensions.Tests.Shared/TestValueObjects/ComplexValueObjectWithDifferentEqualityComparers.cs`.
- Docs: `docs/Smart-Enums-Customization.md`, `docs/Value-Objects-Customization.md`.

## Exact details

For precise accessor and attribute signatures, query **context7** for `Thinktecture.Runtime.Extensions`.
For the operator-generation settings that interact with equality (`SkipEqualityComparison`,
`EqualityComparisonOperators`, `ComparisonOperators`), see `references/value-objects.md`.
