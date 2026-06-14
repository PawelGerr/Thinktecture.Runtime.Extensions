# Generic Types — `TypeParamRef1`–`TypeParamRef5`

C# attributes can't reference an open type parameter directly (`[SmartEnum<T>]` is illegal because `T`
isn't a constant). Thinktecture solves this with **placeholder types** `TypeParamRef1`–`TypeParamRef5`
(namespace `Thinktecture`, five of them) that stand in for the **1-based** type parameters of your own
generic type. The generator substitutes the real type parameter at generation time.

This file is shared by `references/smart-enums.md`, `references/value-objects.md`, and
`references/discriminated-unions.md` — the mechanism is the same in all three families. Load it whenever
the type you are generating is itself **generic** (`Foo<T>`).

```csharp
using Thinktecture;   // TypeParamRef1..5 live here
```

## The one constraint you must not forget

When a `TypeParamRef` is used as the **key type** of a Smart Enum or keyed Value Object, the type
parameter must be non-null: add `where T : notnull` (or `where T : struct` / `where T : class`, which
imply it). Missing it is **TTRESG074** (has a code fix). A generic ad-hoc union that declares type
parameters but never references one via `TypeParamRef` is **TTRESG107**.

## Smart Enum — generic key

```csharp
[SmartEnum<TypeParamRef1>]
public partial class Metric<T>
    where T : INumber<T>
{
    public static readonly Metric<T> Temperature = new(T.Zero);
    public static readonly Metric<T> Humidity    = new(T.One);
}
```

Items remain `public static readonly` fields (closed set, per usual). The generated surface adapts to
what the type parameter's **constraints** prove the key supports (see the table below).

## Value Object — generic key

```csharp
[ValueObject<TypeParamRef1>]
public partial class Identifier<T> where T : notnull;          // class

[ValueObject<TypeParamRef1>]
public readonly partial struct Amount<T> where T : INumber<T>; // struct
```

`Identifier<Guid>.Create(...)`, conversions, equality, etc. are generated as for any keyed VO; parsing,
comparison, and arithmetic appear only when the constraint guarantees them.

### Which constraint generates what

The generator emits an interface/operator on the generated type **only if the type parameter's
constraints prove the key type provides it**:

| Constraint on `T` | Generated on the Smart Enum / Value Object |
|---|---|
| `IParsable<T>` | `IParsable<TSelf>` |
| `ISpanParsable<T>` | `ISpanParsable<TSelf>` (NET9+) |
| `IComparable<T>` | `IComparable<TSelf>` |
| `IFormattable` | `IFormattable` |
| `IComparisonOperators<T,T,…>` | comparison operators (`<` `<=` `>` `>=`) |
| `IAdditionOperators` / `ISubtractionOperators` / `IMultiplyOperators` / `IDivisionOperators` (VO) | the matching arithmetic operators |

Always generated regardless of constraints: factory methods / `Get`·`TryGet`·`Validate`·`Items`,
equality, `Switch`/`Map`, conversions. (Use `where T : INumber<T>` to get parsing + comparison +
arithmetic in one constraint.)

## Ad-hoc union — generic members

Reference type parameters as members, including **nested inside constructed types**:

```csharp
[Union<TypeParamRef1, string>]
public partial struct Result<T>;                       // T-or-string

[Union<TypeParamRef1, List<TypeParamRef1>>]
public partial struct SingleOrMany<T>;                 // nested usage
```

**Type-parameter members get factory methods, not conversion operators** — C# forbids a user-defined
conversion involving an open type parameter. Concrete members keep their implicit conversion:

```csharp
Result<int> ok  = Result<int>.CreateT(42);   // type-param member → factory / ctor only
Result<int> err = "boom";                     // concrete member  → implicit conversion still works
```

Because a type-parameter member is a factory trigger, `Create{Member}` methods are generated for **all**
members (see *Factory methods & their triggers* in `references/discriminated-unions.md`).

Union-specific diagnostics: `TypeParamRef` index past the parameter count → **TTRESG071**; `TypeParamRef`
on a non-generic union → **TTRESG072**; `allows ref struct` type parameter → **TTRESG073** (ref structs
can't be boxed). The `notnull` rule (**TTRESG074**) applies to a type-parameter member used as a key as
above.

### Generic single backing field

`SingleBackingFieldType` accepts `TypeParamRef` placeholders too, so a generic union can type its merged
backing field (and `Value`) as a generic base:

```csharp
[Union<Plain<TypeParamRef1>, Tagged<TypeParamRef1>>(
    SingleBackingFieldType = typeof(IContainer<TypeParamRef1>))]
public partial class Container<T>;   // _obj and Value typed as IContainer<T>, not object
```

See *Memory (advanced)* and *Setting interactions* in `references/discriminated-unions.md` for the
backing-field rules `SingleBackingFieldType` cascades into.

## Common pitfalls

- Omitting `where T : notnull` on a generic key (**TTRESG074**).
- Expecting an implicit conversion **from a type-parameter member** of an ad-hoc union — use the
  generated `Create{Member}` / constructor instead.
- A generic ad-hoc union whose members never use `TypeParamRef` (**TTRESG107**) — drop the generics or
  reference a parameter.
- Forgetting `using Thinktecture;` so `TypeParamRef1` doesn't resolve.

## Worked examples in this repo

Real, compiled examples (CI builds them):

- Smart Enums: `test/Thinktecture.Runtime.Extensions.Tests.Shared/TestEnums/SmartEnum_GenericKeyBased.cs`
  (and `…GenericKeyBasedClassConstraint.cs`, `…GenericKeyBasedStructConstraint.cs`).
- Value Objects: `test/Thinktecture.Runtime.Extensions.Tests.Shared/TestValueObjects/GenericKeyBasedReferenceValueObject.cs`
  (and the `…ClassConstraint` variant).
- Ad-hoc unions: `test/Thinktecture.Runtime.Extensions.Tests.Shared/TestAdHocUnions/` —
  `TestUnion_generic_struct_constrained_TypeParamRef1_string.cs`, `…_array_TypeParamRef1.cs`,
  `…_nested_TypeParamRef1.cs`, `…_dictionary_TypeParamRefs.cs`, `…_duplicate_TypeParamRef1.cs`.
- Docs: `docs/Smart-Enums.md` (*Generic Key Types*), `docs/Value-Objects.md` (generic key types),
  `docs/Discriminated-Unions.md` / `docs/Discriminated-Unions-Customization.md` (generic ad-hoc unions,
  typed single backing field).

## Exact details

For precise constraint→interface mapping and per-family specifics, query **context7** for
`Thinktecture.Runtime.Extensions`, or see the wiki pages linked from the family reference files.
