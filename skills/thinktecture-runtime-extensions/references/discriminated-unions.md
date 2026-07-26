# Discriminated Unions

A value that is **exactly one of several shapes**, with compiler-enforced exhaustive handling. Kills
the `(T?, bool, string?)` tuple / `Result` with nullable fields / exceptions-for-control-flow
patterns.

Two flavors — choose first:

- **Ad-hoc** `[Union<T1,T2>]` / `[AdHocUnion(typeof(T1),typeof(T2))]` — combine *unrelated existing*
  types (`string`, `int`, `List<ValidationError>`) into one "one of" type. Up to 5 members. Quick;
  no shared base.
- **Regular** `[Union]` — a *closed inheritance hierarchy* you own: an abstract base with nested
  sealed cases, each carrying its own data/behavior. Use when the cases are conceptually related.

## Ad-hoc unions

```csharp
[Union<string, int>]
public partial class TextOrNumber;          // class, struct, or ref struct
```

Generated surface:

```csharp
TextOrNumber u = "text";        // implicit conversion from a member type
bool isText = u.IsString;       // Is{Member}
string s = u.AsString;          // As{Member} — throws InvalidOperationException on mismatch
object raw = u.Value;           // untyped
string txt = (string)u;         // explicit cast
// equality, ==/!=, ToString, GetHashCode all delegate to the contained value
```

- **No conversion operator** is generated for `object`, interface, or type-parameter members
  (C# forbids it) — use the constructor or a factory method for those.
- Member names default to the member's type name; rename with `T1Name`/`T2Name`/… (drives `IsX`/`AsX`/
  `CreateX`/`NormalizeX`). A **passed (concrete) type** is named after the type itself — `string` gives
  `IsString`/`AsString`/`CreateString`. A **type-reference** member (`TypeParamRef1`–`TypeParamRef5`) is
  named after the **referenced type parameter**, *not* the ref index — so a union over `<T1, T2>` yields
  `IsT1`/`AsT1`/`CreateT1` and `IsT2`/`AsT2`/`CreateT2`, while one over `<T>` yields `IsT`/`AsT`/`CreateT`
  (and `<TValue>` → `IsTValue`/…).
- `allows ref struct` is **not** supported on ad-hoc member type parameters (TTRESG073) — ref
  structs can't be boxed, which conflicts with `Value`/equality/Switch.
- **Struct unions and `default`:** a `default(TUnion)` struct is uninitialized — reading
  `Value`/`AsX`/`Switch`/`Map` on it throws `InvalidOperationException` (the discriminator is checked
  on every access; it never silently returns `null` or a default). Always create via conversion or
  factory, never `default`.

### Factory methods & their triggers

By default the generator emits `Create{Member}` factory methods for **all** members as soon as **any**
member triggers them. Triggers: a member is a **type parameter**, an **interface**, `System.Object`,
or **duplicates** another member's type. Conversion operators are still generated for eligible
members alongside the factories. Control with `FactoryMethodGeneration`:

- `Default` — auto-detect (above).
- `Always` — generate for all members even without a trigger.
- `None` — suppress all, even for type params/duplicates (you must then supply your own creation).

Factory-method accessibility follows `ConstructorAccessModifier`. Stateless members get
*parameterless* factories.

### Generic ad-hoc unions

Use placeholders `TypeParamRef1`–`TypeParamRef5` (namespace `Thinktecture`) to reference the union's
own type parameters; they also work nested inside constructed types (e.g. `List<TypeParamRef1>`). The
placeholder mechanism, the `notnull` rule, type-param-members-get-factories-not-conversions, and the
`SingleBackingFieldType` generic form are detailed in **`references/generic-types.md`** (shared with
Smart Enums and Value Objects).

```csharp
using Thinktecture;

[Union<TypeParamRef1, string>]
public partial struct Result<T>;

Result<int> ok  = Result<int>.CreateT(42);   // type-param member → factory/ctor (no conversion op)
Result<int> err = "boom";                     // concrete member → implicit conversion still works
```

### Normalizing members

For each non-stateless member the generator emits `static partial void Normalize{Member}(ref TMember value)`.
Implement it to trim/lowercase/replace-null *before* the value is stored — so every observer
(equality, `ToString`, `Switch`/`Map`, `Value`, all serialization round-trips) sees the normalized
value. Optional; erased by the compiler if you don't implement it.

### Memory (advanced)

By default the generator merges **2+ reference-type members** into one `object? _obj` field (value
types stay typed). Override with `UseSingleBackingField` (force all members in, boxing value types),
`SingleBackingFieldType` (type `_obj`/`Value` as a base), or **stateless members**
(`TXIsStateless = true`, discriminator-only — prefer structs). When no member keeps its own typed
field, `Value` returns `_obj` directly instead of selecting by discriminator; this changes the
generated code only, not the behaviour. See *Setting interactions* below for the forcing rules and
`TTRESG075`.

### Serialization

Ad-hoc unions carry **no discriminator**, so they aren't polymorphic-serializable out of the box.
Add `[ObjectFactory<T>]` (usually `<string>`) to define a single-value representation for JSON / model
binding / EF — see **`references/object-factories.md`**. For how the chosen representation is then
wired into each framework, see `references/framework-integration.md`.

## Regular unions

```csharp
[Union]
public abstract partial record OrderState     // base is partial (abstract recommended)
{
    public sealed record Pending(DateTime CreatedAt) : OrderState;
    public sealed record Shipped(DateTime ShippedAt, string Tracking) : OrderState;
}
```

- Derived cases must be **nested** in the base. The nesting rules differ by kind:
  - **Records**: every derived record must be `sealed` (TTRESG055), so records **cannot nest** deeper
    than one level. Use classes if you need a multi-level hierarchy.
  - **Classes**: a `sealed` leaf needs nothing special; a **non-sealed intermediate** (e.g. `Failure`
    in `ApiResponse > Failure > NotFound|Unauthorized`) must have **only private constructors**
    (TTRESG054). Multi-level nesting is then supported.
  - Derived types must not be generic (TTRESG053) and must not be *less* accessible than the base
    (TTRESG056).
- When the base is itself nested in another type, every **enclosing** type must also be `partial`
  (TTRESG006).
- **Data-free cases are just empty `sealed record`s** (e.g. `Success`, `NotFound`). The
  `TXIsStateless` flag is an *ad-hoc-union* setting and has no role here.
- The generator emits an implicit conversion for each unique constructor-parameter type, plus
  `Switch`/`Map`.
- Conversion-operator generation is controlled by `ConversionFromValue` (`None`/`Implicit`/`Explicit`).
  Regular unions have **no** `ConstructorAccessModifier` — that is an *ad-hoc-union* setting.
- Per-case behavior: either abstract methods on the base (when behavior is inherent, e.g.
  `CanCancel()`) or an external `Switch` (preferred for transitions / cross-cutting concerns).
- **Serialization is polymorphic**: for System.Text.Json apply `[JsonDerivedType]` on the base ($type
  discriminator); Newtonsoft.Json uses `TypeNameHandling` ($type, **no integration package needed**);
  for EF Core use standard TPH/TPT. (An `[ObjectFactory<T>]` is an alternative single-value route.)
  Registration mechanics are in `references/framework-integration.md`.
- For nested hierarchies, generate extra `Switch`/`Map` overloads via `[UnionSwitchMapOverload]`. Its
  `StopAt` parameter chooses the granularity: the overload treats the named type(s) as leaves (handling
  them and their siblings) instead of descending to the deepest cases — so you can match at
  `Success | Failure` and delegate `Failure`'s sub-cases to a second, deeper overload.

## Pattern matching: prefer `Switch`/`Map` over native `switch`

Native `switch` needs a `_ => …` arm that silently swallows newly added cases; generated `Switch`
(side effects) and `Map` (value per case) are **exhaustive**, so adding a case is a compile error at
every call site. Full guidance — exhaustiveness, `SwitchPartially`/`MapPartially`, the
`[UnionSwitchMapOverload]` overloads for nested hierarchies, static-lambda performance, explicit
`TResult`, and the IDE light-bulb completion — is shared with Smart Enums in
**`references/switch-map.md`**.

## Setting interactions

Several settings silently force others:

- `TXIsStateless = true` → auto-sets `TXIsNullableReferenceType = true` for reference types.
- `ConstructorAccessModifier` (**ad-hoc unions only**) → also sets accessibility of the generated
  conversion operators **and** factory methods. (Regular unions instead control conversion operators
  via `ConversionFromValue`; ad-hoc unions also have `ConversionToValue`.)
- `SingleBackingFieldType = typeof(TBase)` → implies `UseSingleBackingField = true` (conflict with
  explicit `false` is `TTRESG075`).
- `FactoryMethodGeneration` → `None` suppresses all factories (even type-param/duplicate members);
  `Always` generates for all; `Default` generates for all when any trigger is present.

## Common pitfalls

- Forgetting `partial` (base type for regular unions).
- Expecting conversion operators for `object`/interface/type-parameter members — use ctor/factory.
- Serializing an ad-hoc union without an `[ObjectFactory<T>]`.
- Using native `switch` with `_ =>` and losing exhaustiveness.
- `allows ref struct` on an ad-hoc type parameter (TTRESG073).
- Reading `Value`/`Switch`/`Map` on a `default` struct union — it throws `InvalidOperationException`.

## Worked examples in this repo

Real, compiled examples (correct by construction — CI builds them):

- Samples: `samples/Basic.Samples/Unions/` — regular (`Result.cs`, `ApiResponse.cs`, `Animal.cs`),
  ad-hoc (`TextOrNumber.cs`, `PartiallyKnownDate.cs`), serializable (`TextOrNumberSerializable.cs`);
  driver `Unions/DiscriminatedUnionsDemos.cs`.
- Exhaustive variants: `test/Thinktecture.Runtime.Extensions.Tests.Shared/TestAdHocUnions/` and
  `.../TestRegularUnions/`.
- Docs: `docs/Discriminated-Unions.md`, `docs/Discriminated-Unions-Customization.md`.

## Exact details

For precise property names/defaults (`T1Name`, `TXIsStateless`, `FactoryMethodGeneration`,
`SingleBackingFieldType`, `UnionSwitchMapOverload`, nested parameter-name modes) query **context7**
for `Thinktecture.Runtime.Extensions`, or see https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Discriminated-Unions and
https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Discriminated-Unions-Customization. For serialization helpers see
`references/object-factories.md`.

For Smart Enums see `references/smart-enums.md`; for Value Objects see `references/value-objects.md`;
for pattern matching see `references/switch-map.md`; for serializer/EF/ASP.NET wiring see
`references/framework-integration.md`; for generic (`Foo<T>`) unions see `references/generic-types.md`;
for diagnostics see `references/diagnostics.md`.
