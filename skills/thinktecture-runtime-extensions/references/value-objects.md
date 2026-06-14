# Value Objects

An immutable domain primitive that wraps one or more underlying values, validates them at creation,
and gives them a distinct type. Cures primitive obsession: `Amount`, `Email`, `ISBN`, `Boundary`
instead of bare `decimal`/`string`.

Two flavors:

- **Simple** `[ValueObject<TKey>]` — wraps a single key value (`string`, `int`, `decimal`, `Guid`…).
  Gets conversion operators to/from the key, parsing, comparison.
- **Complex** `[ComplexValueObject]` — wraps several read-only properties that form one concept
  (e.g. `Boundary { Lower, Upper }`). No single key; equality is structural across members.

Value Objects vs Smart Enums: a Smart Enum is a *fixed, known set* of instances; a Value Object is
an *open set* validated at creation. If you can't enumerate the valid values up front, it's a Value
Object.

## Basic shape

```csharp
[ValueObject<string>]                 // simple
public partial class ProductName;     // generator adds 'sealed' for classes, 'readonly' for structs

[ComplexValueObject]                  // complex
public partial class Boundary
{
    public decimal Lower { get; }     // read-only props set via the generated constructor
    public decimal Upper { get; }
}
```

You always get: `Create` / `TryCreate` / `Validate` factory methods, equality
(`Equals`/`GetHashCode`/`==`/`!=`), `ToString`. Simple VOs additionally get conversion operators and
— when the key type supports it — `IParsable`/`ISpanParsable`/`IComparable`/`IFormattable`.
**Don't hand-write these.**

## Validation & normalization — prefer `ValidateFactoryArguments`

Provide the body of the generated `partial` method `ValidateFactoryArguments`. It runs inside the
factory methods (`Create`/`TryCreate`/`Validate`) and on every framework read path (JSON, model
binding, EF Core), so validation and normalization happen consistently everywhere.

```csharp
[ValueObject<string>]
public partial class ProductName
{
    static partial void ValidateFactoryArguments(ref ValidationError? validationError, ref string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            validationError = new ValidationError("Product name cannot be empty");
            return;
        }
        value = value.Trim();           // ref param → normalize in place
    }
}
```

Two rules that matter:

- **Set `validationError`; don't throw.** Throwing breaks JSON/model-binding/EF integration. The
  `ref ValidationError?` signature also lets the compiler erase the method entirely when you provide
  no body — zero overhead for VOs without validation.
- **Parameters are `ref`** so you can normalize (trim, round, lowercase). Normalization here is seen
  by equality, serialization, and persistence alike.

`ValidateConstructorArguments` also exists (runs in the constructor) but can only throw — prefer the
factory variant. Use the constructor variant only when you specifically need constructor-time
enforcement.

### Additional context parameters (and `CreateCore`/`ValidateCore`)

You can add **trailing parameters** to `ValidateFactoryArguments` to flow extra context into the
hook (e.g. a rounding strategy). Declare them **without** a default value (the generator supplies the
default on its own declaration; a default on yours is `CS1066`, and `ref`/`out`/defaults are
`TTRESG076`). The generator then emits two helpers you delegate to from hand-written factories:

- **`CreateCore(value, extras…)`** — throwing building block (mirrors `Create`). Its name tracks
  `CreateFactoryMethodName`, so if you rename the factory (e.g. `CreateFactoryMethodName = "Of"`) the
  building block becomes `OfCore`.
- **`ValidateCore(value, extras…, out T? obj)`** — non-throwing; returns `ValidationError?`.

```csharp
[ValueObject<decimal>]
public partial struct Money
{
    static partial void ValidateFactoryArguments(
        ref ValidationError? validationError, ref decimal value, MoneyRoundingStrategy? roundingStrategy)
    {
        if (value < 0) { validationError = new ValidationError("Amount cannot be negative"); return; }
        value = (roundingStrategy ?? MoneyRoundingStrategy.Default).Round(value);   // runs exactly once
    }

    // Hand-written overloads collapse to one-liners over the generated building block:
    public static Money Create(decimal amount, MoneyRoundingStrategy strategy) => CreateCore(amount, strategy);
}
```

**Always delegate to `CreateCore`/`ValidateCore`** rather than re-implementing the plumbing. The
generator emits both — as **`private static`** — whenever the hook has additional parameters,
*regardless of the hook's return type*, so they are callable only from within the type's own
`partial` declaration. A hand-rolled factory would re-run the plumbing incorrectly (and silently drop
the hook's return value when you use the non-`void` variant). The public `Create`/`TryCreate`/
`Validate` signatures stay unchanged, so framework integration is unaffected.

## Composing value objects

A complex VO's members can themselves be Value Objects (or Smart Enums) — each enforces its own
rules, and the complex VO adds **cross-component** validation in `ValidateFactoryArguments` (one `ref`
parameter per member, in declaration order):

```csharp
[ComplexValueObject]
public partial struct Period
{
    public DateOnly From { get; }
    public OpenEndDate Until { get; }              // member is itself a Value Object

    static partial void ValidateFactoryArguments(
        ref ValidationError? validationError, ref DateOnly from, ref OpenEndDate until)
    {
        if (from >= until)
            validationError = new ValidationError("From must be earlier than Until");
    }
}
```

Sample: `samples/Basic.Samples/ValueObjects/Period.cs` (composes the `OpenEndDate` VO). A fuller
`Address` example (`Street`/`City`/`PostalCode` + a `CountryCode` Smart Enum, validated together) is
in `docs/Value-Objects.md` (*Composing Value Objects*).

## String keys need an equality comparer

For a `[ValueObject<string>]`, specify how strings compare — the analyzer warns otherwise. Default
(no attribute) is `OrdinalIgnoreCase`.

```csharp
[ValueObject<string>]
[KeyMemberEqualityComparer<ComparerAccessors.StringOrdinal, string>]   // exact, case-sensitive
public sealed partial class ProductName;
```

Predefined accessors: `ComparerAccessors.StringOrdinal(IgnoreCase)`, `…CurrentCulture(IgnoreCase)`,
`…InvariantCulture(IgnoreCase)`, `Default<T>`. For complex VOs use `[MemberEqualityComparer<…>]` on
individual properties (and `DefaultStringComparison` on the attribute); exclude a property from
generated equality/factory with `[IgnoreMember]`. For ordering (not equality) on simple VOs use
`[KeyMemberComparer<…>]`. Full comparer rules (the attributes' arity, the complete `ComparerAccessors`
list, equality-vs-ordering, and the TTRESG048/049/102/103 warnings) are shared with Smart Enums in
**`references/equality-and-comparers.md`**.

## Structs: default value is disallowed

A struct VO rejects `default(T)` and `new()` (TTRESG047) because that bypasses validation. Opt in
with `AllowDefaultStructs = true`, which generates a static `Empty` member (rename via
`DefaultInstancePropertyName`). Not allowed when the key is a reference type (TTRESG057) or a member
itself disallows defaults (TTRESG058).

## Custom / existing key member

By default the generator creates the backing key member for a simple VO. Tune it with `KeyMemberName`
(its name), `KeyMemberKind` (property vs. field), and `KeyMemberAccessModifier` (its accessibility).
Set `SkipKeyMember = true` to **implement the key member yourself** (the generator only consumes it) —
pair it with `KeyMemberName` so the generator knows which member is the key:

```csharp
[ValueObject<DateOnly>(SkipKeyMember = true,        // we implement the key member ourselves
                       KeyMemberName = nameof(Date))]
public partial struct OpenEndDate
{
    // hand-written key member named "Date" — see the sample for the full body
}
```

Sample: `samples/Basic.Samples/ValueObjects/OpenEndDate.cs`.

## Conversion operators (simple VOs)

Defaults: `ConversionToKeyMemberType = Implicit`, `ConversionFromKeyMemberType = Explicit`. Tune per
direction with those properties plus `UnsafeConversionToKeyMemberType` (reference-type VO → value-type
key). Each is `None`/`Implicit`/`Explicit`.

## Operators (comparison, equality, arithmetic)

Beyond conversion operators, the generator can emit **comparison** (`<` `<=` `>` `>=`), **equality**
(`==` `!=`), and **arithmetic** (`+` `-` `*` `/`) operators. Each is controlled by an
`OperatorsGeneration` value:

- `None` — not generated.
- `Default` — operator between two value objects (e.g. `Amount + Amount`).
- `DefaultWithKeyTypeOverloads` — also overloads with the key type (e.g. `Amount + decimal`).

```csharp
[ValueObject<decimal>(
    ComparisonOperators  = OperatorsGeneration.DefaultWithKeyTypeOverloads,   // < <= > >=
    AdditionOperators    = OperatorsGeneration.DefaultWithKeyTypeOverloads,   // +
    SubtractionOperators = OperatorsGeneration.Default,                       // -
    MultiplyOperators    = OperatorsGeneration.None,                          // (no *)
    DivisionOperators    = OperatorsGeneration.None)]                         // (no /)
public readonly partial struct Amount;
```

- Equality operators are controlled by `EqualityComparisonOperators`; comparison interfaces by
  `SkipIComparable`. These interact — `ComparisonOperators` coerces `EqualityComparisonOperators`
  upward, and `SkipEqualityComparison = true` forces both to `None` (see the cascade table below).
- Arithmetic generates **both** checked and unchecked operator forms.

## Generic, null/empty, and custom-error options

- **Generic key** (`Identifier<T>`): use the `TypeParamRef` placeholder and a `notnull` constraint —
  see `references/generic-types.md` (shared with Smart Enums and unions; the constraint table there
  decides which interfaces/operators are generated).
- **Optional primitives**: `NullInFactoryMethodsYieldsNull = true` makes the factory return `null`
  instead of erroring on a `null` input; `EmptyStringInFactoryMethodsYieldsNull = true` treats
  empty/whitespace as `null` (and forces the former — see the cascade table). Note
  `EmptyStringInFactoryMethodsYieldsNull` applies only to **string-keyed** VOs (silently ignored
  otherwise). Use these to model "absent" string VOs cleanly rather than throwing.
- **Custom validation errors**: to return a richer error than the default `ValidationError`, implement
  `IValidationError<T>` on your error type — referencing itself, i.e.
  `class MyError : IValidationError<MyError>` (the interface is `IValidationError<out T> where T : class`).
  It declares only `static abstract T Create(string message)`; you must **also override `object.ToString()`**
  (framework integration formats the error via `ToString()`, but it is not an interface member). Reference
  the type via `[ValidationError<T>]`; it then becomes the type of the `validationError` parameter in your hook:

  ```csharp
  public class BoundaryValidationError : IValidationError<BoundaryValidationError>
  {
      public string Message { get; }
      public BoundaryValidationError(string message) => Message = message;
      public static BoundaryValidationError Create(string message) => new(message);
      public override string ToString() => Message;
  }

  [ComplexValueObject]
  [ValidationError<BoundaryValidationError>]
  public partial class Boundary
  {
      public decimal Lower { get; }
      public decimal Upper { get; }

      static partial void ValidateFactoryArguments(
          ref BoundaryValidationError? validationError, ref decimal lower, ref decimal upper)
      {
          if (lower > upper)
              validationError = new BoundaryValidationError("Lower must be <= Upper.");
      }
  }
  ```

  Sample: `samples/Basic.Samples/ValueObjects/Boundary.cs` + `BoundaryValidationError.cs` (the real
  error type also carries the offending `Lower`/`Upper` values).

## Custom conversion / serialization

For *how* serialization / model binding / EF registration is wired (packages vs manual converter
registration) see **`references/framework-integration.md`**.

For a *custom* (de)serialization format, model binding of non-string keys, or single-column EF
persistence of a complex VO, add `[ObjectFactory<T>]` — see **`references/object-factories.md`**. Note: if
`SkipFactoryMethods = true` would suppress serialization converters, an `[ObjectFactory<T>]` with
`UseForSerialization` re-enables them.

## Setting cascade (account for the whole chain)

Several settings silently force others — never reason about one in isolation:

| Setting | Forces |
|---|---|
| `SkipFactoryMethods = true` | Skips `IParsable`/`ISpanParsable`; arithmetic operators → `None`; suppresses `TypeConverter`, `IObjectFactory<T>`, the key-type conversion operator, **and serialization converters** (re-enable serialization with `[ObjectFactory<T>(UseForSerialization = …)]`) |
| `SkipIParsable = true` | `SkipISpanParsable = true` (`ISpanParsable<T>` inherits `IParsable<T>`) |
| `SkipEqualityComparison = true` | `ComparisonOperators` **and** `EqualityComparisonOperators` → `None` |
| `EqualityComparisonOperators = None` | `ComparisonOperators` → `None` (comparison needs equality) |
| `ComparisonOperators` > `EqualityComparisonOperators` | `EqualityComparisonOperators` coerced upward to match |
| `EmptyStringInFactoryMethodsYieldsNull = true` | `NullInFactoryMethodsYieldsNull = true` |

## Common pitfalls

- Forgetting `partial` — including every **enclosing** type when the VO is nested (TTRESG006).
- Throwing in `ValidateFactoryArguments` instead of setting `validationError`.
- Missing `[KeyMemberEqualityComparer]` on a string VO (analyzer warning).
- Re-implementing factory plumbing instead of delegating to `CreateCore`/`ValidateCore`.
- Using `default(struct VO)` without `AllowDefaultStructs`.
- Forgetting the `SkipFactoryMethods` cascade (see above) — it silently turns off serialization/parsing.

## Worked examples in this repo

Real, compiled examples (correct by construction — CI builds them):

- Samples: `samples/Basic.Samples/ValueObjects/` (e.g. simple `Amount.cs`, `ProductName.cs`,
  `Money.cs`; complex `Boundary.cs`, `BoundaryWithFactories.cs`, `Period.cs`); driver
  `ValueObjects/ValueObjectDemos.cs`.
- Exhaustive variants: `test/Thinktecture.Runtime.Extensions.Tests.Shared/TestValueObjects/`.
- Docs: `docs/Value-Objects.md`, `docs/Value-Objects-Customization.md`.

## Exact details

For precise property names/defaults (`KeyMemberName`, `ConstructorAccessModifier`, operator
generation enums, `NullInFactoryMethodsYieldsNull`, etc.) query **context7** for
`Thinktecture.Runtime.Extensions`, or see https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Value-Objects and
https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Value-Objects-Customization.

For Smart Enums see `references/smart-enums.md`; for one-of types see `references/discriminated-unions.md`;
for serializer/EF/ASP.NET wiring see `references/framework-integration.md`; for generic (`Foo<T>`) VOs
see `references/generic-types.md`; for comparers see `references/equality-and-comparers.md`; for
diagnostics see `references/diagnostics.md`.
