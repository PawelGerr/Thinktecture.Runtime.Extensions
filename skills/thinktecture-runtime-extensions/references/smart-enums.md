# Smart Enums

A type-safe enum that can carry data and behavior. Unlike a plain `enum`, items are full objects,
the set is closed and validated, and you cannot cast an arbitrary number into an invalid value.

Two variants:

- **Keyed** `[SmartEnum<TKey>]` — each item has an underlying key (`int`, `string`, `Guid`, custom).
  Enables lookup by value, parsing, and serialization round-trips.
- **Keyless** `[SmartEnum]` — no underlying value; items are identified only by field reference.

## Basic shape

```csharp
[SmartEnum<string>]
public partial class ShippingMethod
{
    public static readonly ShippingMethod Standard = new("STANDARD");
    public static readonly ShippingMethod Express   = new("EXPRESS");
}
```

- The type is `partial`. The generator adds the key property, `Get`/`TryGet`, equality,
  conversions, `Switch`/`Map`, and (keyed) parsing & serialization.
- Items are `public static readonly` fields. There is **no on-demand creation** — the set is fixed
  at compile time.
- **You don't write the constructor.** The generator emits a `private` constructor whose parameters
  are the key plus every declared instance property/field, and assigns them. Add/remove/reorder
  members and it is regenerated. (Write a constructor only for inheritance scenarios.)

## The core design principle

**Store per-item data in declared fields/properties, set via the item initializers. Never branch on `this`.**

The whole point of a Smart Enum is that each item carries its own data and behavior. Putting
`if (this == X)` / `switch` on `this` inside instance methods recreates the plain-enum problem you
were trying to escape.

```csharp
// GOOD — data travels with the item
[SmartEnum<string>]
public partial class ShippingMethod
{
    public static readonly ShippingMethod Standard = new("STANDARD", baseCost: 5m, maxDays: 7);
    public static readonly ShippingMethod Express   = new("EXPRESS",  baseCost: 15m, maxDays: 2);

    public decimal BaseCost { get; }   // declared members become constructor
    public int MaxDays { get; }        // parameters automatically — no ctor to write
}
```

The `new("STANDARD", baseCost: 5m, maxDays: 7)` calls the **generated** constructor
`(string key, decimal baseCost, int maxDays)`. For per-item *behavior* (not data), declare a
`partial` method with `[UseDelegateFromConstructor]` and pass a delegate per item, or use inheritance.

```csharp
// BAD — anti-pattern, defeats the purpose
public decimal GetCost() => this == Standard ? 5m : this == Express ? 15m : 0m;
```

## Inter-item references: use Lazy<T>

Referencing another item directly during static field initialization fails due to initialization
order — when `Pending` is constructed, `Shipped` may not exist yet. Store the reference in a
`Lazy<T>` **field** (a generated-constructor parameter); the lambda runs on first access, by which
time all items exist:

```csharp
[SmartEnum<string>]
public partial class OrderStatus
{
    public static readonly OrderStatus Pending   = new("Pending",   new(() => [Processing, Shipped]));
    public static readonly OrderStatus Processing = new("Processing", new(() => [Shipped]));
    public static readonly OrderStatus Shipped   = new("Shipped",   new(() => [Delivered]));
    public static readonly OrderStatus Delivered = new("Delivered", new(() => []));   // terminal

    private readonly Lazy<IReadOnlyList<OrderStatus>> _nextStates;   // generated ctor parameter

    public bool CanTransitionTo(OrderStatus next) => _nextStates.Value.Contains(next);
}
```

`new(() => [Processing, Shipped])` is a target-typed `Lazy<IReadOnlyList<OrderStatus>>` whose factory
returns the transitions; the generator threads the field through the constructor — again, **no
hand-written constructor**.

## Validation & lookup (keyed)

Keyed enums generate lookup and validation entry points over the fixed `Items` set:

```csharp
ProductType x = ProductType.Get("Electronics");            // throws if no such key
bool ok       = ProductType.TryGet("Electronics", out var item);   // false if unknown
ValidationError? e = ProductType.Validate("Electronics", null, out var validated); // null on success
```

- `Get` throws on an unknown key; `TryGet` returns `false`; `Validate` returns a `ValidationError`
  (used by the framework read paths). The conversion cast `(ProductType)"Electronics"` is equivalent
  to `Get`.
- **To validate/normalize the per-item construction arguments**, implement the `partial` method
  `ValidateConstructorArguments` (the key **and every extra per-item field** arrive by `ref`, so you
  can trim/normalize any of them in place). Unlike Value Objects, Smart Enums use the **constructor** hook —
  there is no `ValidateFactoryArguments` here, because items are fixed static fields, not created on
  demand. The key must never be `null`.

```csharp
[SmartEnum<string>]
public partial class ProductType
{
    public static readonly ProductType Electronics = new("Electronics");

    static partial void ValidateConstructorArguments(ref string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new Exception("Key cannot be empty.");
        key = key.Trim();
    }
}
```

## Pattern matching: Switch / Map

`Switch`/`Map` (and `SwitchPartially`/`MapPartially`) are generated and **exhaustive** — prefer them
over `if`/`switch` at call sites. The full guidance (exhaustiveness, static-lambda performance, the
IDE light-bulb completion) is shared with unions in **`references/switch-map.md`** — read it there.

## What the generator gives you

- Keyed & keyless: equality (`Equals`/`GetHashCode`/`==`/`!=`), `Switch`/`Map`, an `Items`
  collection.
- Keyed additionally: the key property, `Get`/`TryGet` lookup, conversion operators, and —
  depending on what the key type supports — `IParsable<T>`, `ISpanParsable<T>` (NET9+),
  `IComparable<T>`, `IFormattable`, plus serializer integration when an integration package is
  referenced (registration mechanics in `references/framework-integration.md`).
- String-keyed enums on NET9+ get span-based JSON deserialization automatically (opt out with
  `DisableSpanBasedJsonConversion = true`).

For how serialization / model binding / EF registration is actually wired up (packages vs manual
converter registration), see **`references/framework-integration.md`**.

**Keyless enums have no key**, so there is nothing to serialize or bind from. For JSON, model
binding, or EF on a `[SmartEnum]` (keyless) you **must** add `[ObjectFactory<T>]` — it is the *only*
route. See `references/object-factories.md` (real example: `test/Thinktecture.Runtime.Extensions.Tests.Shared/TestEnums/SmartEnum_Keyless_ObjectFactory.cs`).

## Inheritance & nesting (keyed)

- Derived types must be **nested inner classes**.
- First-level nested types must be `private` (TTRESG014); deeper levels must be `public` (TTRESG015).
- The generator auto-seals only the **top-level** enum's own generated `partial` part when it has no
  derived types, so you never write `sealed` on the root yourself. This does **not** extend to the
  inner derived classes — they aren't `partial`, so the generator can't touch them. Each **leaf**
  derived item must be declared `sealed` by you; a non-`sealed`, non-`abstract` leaf raises TTRESG037
  (an **error**, with a code-fix). Derived classes that themselves have further derived types stay open.
- **When the Smart Enum is itself nested in another type, every enclosing type must also be `partial`**
  (TTRESG006) — a common "nothing generated" cause.

## Base class support

A Smart Enum may inherit from an ordinary (non-enum) base class to share data/behavior. You still
**don't write the constructor** — the generator emits one whose parameters are, in order: **the key,
then the enum's own declared members, then the base class's constructor parameters**. Pass them all in
the item initializer in that order.

```csharp
public class SomeBaseClass(int value) { public int Value { get; } = value; }

[SmartEnum<string>]
public partial class EnumWithBaseClass : SomeBaseClass
{
    // generated ctor: (string key, string description, int value)
    public static readonly EnumWithBaseClass Item1 = new("item 1", "Item 1 Description", 42);
    public string Description { get; }   // own member → comes before the base's `value`
}
```

Sample: `samples/Basic.Samples/SmartEnums/EnumWithBaseClass.cs`.

## Custom key member

The generator exposes the key as a property derived from the `[SmartEnum<TKey>]` argument. Tune it
with `KeyMemberName` (the property name), `KeyMemberKind` (property vs. field), and
`KeyMemberAccessModifier` (its accessibility). Useful when the default name/visibility doesn't fit
the surrounding code (e.g. exposing the key as `Code` instead of the default).

## Generic & string-keyed specifics

- **Generic key** (`Foo<T>`): use the `TypeParamRef` placeholder and a `notnull` constraint — see
  `references/generic-types.md` (the mechanism is shared with Value Objects and unions).
- **String key**: comparison defaults to `OrdinalIgnoreCase`. To override (e.g. case-sensitive codes)
  or to add ordering, see `references/equality-and-comparers.md`. On NET9+ a hand-written comparer without
  `IAlternateEqualityComparer<ReadOnlySpan<char>, string>` makes the span-based `Get`/`TryGet` fall back to the
  string lookup, which allocates one string per call.

## Common pitfalls

- Forgetting `partial` → nothing generates.
- Branching on `this` instead of storing per-item fields.
- Direct cross-item references in constructors without `Lazy<T>`.
- Expecting to create new items at runtime — the set is fixed.
- Hand-writing the constructor after declaring properties/fields — it conflicts with the generated
  one. Just declare the members and pass their values in the item initializers.
- Hand-writing equality or `Switch`/`Map` — let the generator emit them.
- Expecting a **keyless** enum to serialize / model-bind without an `[ObjectFactory<T>]` — it can't;
  there is no key to convert from.

## Worked examples in this repo

Real, compiled examples (correct by construction — CI builds them):

- Samples: `samples/Basic.Samples/SmartEnums/` (e.g. `ProductType.cs`, `MoneyRoundingStrategy.cs`,
  `EnumWithBaseClass.cs`, keyless `OperatorWithoutIdentifier.cs`); driver `SmartEnums/SmartEnumDemos.cs`.
- Exhaustive variants: `test/Thinktecture.Runtime.Extensions.Tests.Shared/TestEnums/`.
- Docs: `docs/Smart-Enums.md`, `docs/Smart-Enums-Customization.md`, `docs/Smart-Enums-Performance.md`.

## Exact details

For precise attribute properties (e.g. `KeyMemberName`, `KeyMemberKind`, `KeyMemberAccessModifier`,
comparer attributes) and their defaults, query **context7** for `Thinktecture.Runtime.Extensions`, or see
https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Smart-Enums, https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Smart-Enums-Customization, and https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Smart-Enums-Performance.

For Value Objects see `references/value-objects.md`; for one-of types see `references/discriminated-unions.md`;
for pattern matching see `references/switch-map.md`; for serializer/EF/ASP.NET wiring see
`references/framework-integration.md`; for generic (`Foo<T>`) enums see `references/generic-types.md`;
for string/custom comparers see `references/equality-and-comparers.md`; for diagnostics see
`references/diagnostics.md`.
