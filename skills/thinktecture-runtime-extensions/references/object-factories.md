# Object Factories — `[ObjectFactory<T>]`

A custom, bidirectional bridge between your type and some other type `T`. Applying
`[ObjectFactory<T>]` makes the generator add `IObjectFactory<YourType, T, ValidationError>` (you
implement a static `Validate`) and, when used for writing, `IConvertible<T>` (you implement
`ToValue()`). When `T` is `string`, you also get `IParsable<YourType>` for free.

## The headline capability: it works on plain classes

`[ObjectFactory<T>]` is **not limited to the library's generated types**. Put it on an ordinary
`partial class` (no `[SmartEnum]`/`[ValueObject]`/`[Union]`) and you get parsing, model binding,
serialization, and EF Core persistence for a type the generator otherwise knows nothing about. The
only requirement is `partial` plus the `Validate`/`ToValue` methods.

```csharp
[ObjectFactory<string>(UseWithEntityFramework = true)]
public partial class CustomObject   // plain class — no other TTRE attribute
{
    public required string Property1 { get; init; }
    public required string Property2 { get; init; }

    public static ValidationError? Validate(string? value, IFormatProvider? provider, out CustomObject? item)
    {
        if (value is null) { item = null; return null; }
        var parts = value.Split('|');
        item = new() { Property1 = parts[0], Property2 = parts[1] };
        return null;
    }

    public string ToValue() => $"{Property1}|{Property2}";
}
```

This is the most powerful and least-known use: a single `Validate`+`ToValue` pair gives an arbitrary
class one consistent representation across JSON, model binding, and the database — without making it
a Smart Enum / Value Object / Union.

## The contract

- **`Validate`** (always required): `static ValidationError? Validate(T? value, IFormatProvider? provider, out YourType? item)`.
  Return `null` on success (and set `item`), or a `ValidationError` describing the failure (TTRESG061
  if missing/mis-signed).
- **`ToValue()`** (required only for *writing*): `public T ToValue()`. Needed when
  `UseForSerialization != None` or `UseWithEntityFramework = true` (TTRESG062 otherwise). Without it
  you have a one-way `T → YourType` conversion only.

## One-way vs two-way

- **One-way** (`Validate` only): consume `T` to build your type — e.g. accept extra string forms in
  addition to the normal key. Good for parsing/model-binding-in only.
- **Two-way** (`Validate` + `ToValue`): full round-trip for serialization and/or EF Core.

## Framework integration flags

All four default to **off** (`UseForSerialization = None`; the bools `false`), so each integration
point is opt-in — setting none of them gives a `Validate`/`ToValue` pair that no framework uses.

| Property | Default | Effect |
|---|---|---|
| `UseForSerialization` (`SerializationFrameworks`) | `None` | Which serializers use this factory: `None`/`SystemTextJson`/`NewtonsoftJson`/`Json`/`MessagePack`/`All`. For keyed types this *replaces* key-based conversion. |
| `UseForModelBinding` (bool) | `false` | ASP.NET Core binds via `T`. Essential for non-string-keyed or keyless types — pick `[ObjectFactory<string>]` so route/query strings bind. Register `ThinktectureModelBinderProvider` for MVC controllers. |
| `UseWithEntityFramework` (bool) | `false` | EF Core persists via `T` (single column). Requires `ToValue()`. Register converters with `UseThinktectureValueConverters()` / `AddThinktectureValueConverters()`. |
| `HasCorrespondingConstructor` (bool) | `false` | Tells EF Core a single-`T` constructor exists; **EF Core uses it on DB load to bypass validation** (DB is source of truth). This is also the **EF performance** lever: without it, `Validate` re-runs on *every* row materialization, so set it (with a matching ctor) when validation is expensive and the DB is trusted. Does NOT affect JSON/MessagePack/Newtonsoft/model binding — those always go through `Validate`. Not allowed on Smart Enums (TTRESG060); the constructor must actually exist (TTRESG059). |

A single `[ObjectFactory<string>]` can carry several flags at once (e.g. `UseForSerialization`,
`UseForModelBinding`, **and** `UseWithEntityFramework`) — one `Validate`/`ToValue` pair then serves
all of them. Split across multiple factories only when different integration points need a different
`T` (see *Multiple factories* below).

## Multiple factories

Apply several `[ObjectFactory<T>]` with different `T` (e.g. `string` for JSON+binding, `byte[]` for
MessagePack). Implement one `Validate` (and `ToValue`) per `T`. Conflict rules (each integration
point must be unique):

- only one factory with `UseWithEntityFramework = true` (TTRESG068)
- only one factory with `UseForModelBinding = true` (TTRESG069)
- no overlapping serialization frameworks across factories (TTRESG070)

## Zero-allocation JSON (.NET 9+)

`[ObjectFactory<ReadOnlySpan<char>>(UseForSerialization = SerializationFrameworks.SystemTextJson)]`
lets System.Text.Json transcode UTF-8 JSON directly to a `ReadOnlySpan<char>` and call your
span-based `Validate` — no intermediate `string`. Pattern-match the span against string constants
for known values to allocate nothing; fall back to `value.ToString()` only for unknown values. Use
`SystemTextJson` specifically (only it supports span deserialization); the converter `stackalloc`s
up to 128 chars and rents from `ArrayPool<char>.Shared` beyond that. `ToValue()` returns
`ReadOnlySpan<char>`. A `ReadOnlySpan<char>` factory must not enable `UseWithEntityFramework` or
`UseForModelBinding` (TTRESG078), because a ref struct cannot be the generic value type of the
EF Core value converter or the ASP.NET Core model binder.

## Type-specific notes

- **Keyless Smart Enums** (`[SmartEnum]`): `[ObjectFactory<T>]` is the *only* way to get
  serialization/model binding (no key type to convert from).
- **String-keyed Smart Enums**: already get zero-alloc JSON on NET9+ automatically; a span factory
  is redundant unless customizing the format.
- **Value Objects**: works on simple and complex. If `SkipFactoryMethods = true` would suppress
  serialization converters, a factory with `UseForSerialization` re-enables them.
- **Ad-hoc unions**: no discriminator, so not polymorphic-serializable — an object factory (usually
  `<string>`) is the primary way to serialize them as a single value.
- **Regular unions**: can use EF inheritance (TPH/TPT), but a factory offers single-value
  serialization as an alternative.

## Metadata priority (important gotcha)

When a type has **both** a key (`[SmartEnum<TKey>]`/`[ValueObject<TKey>]`) and an
`[ObjectFactory<T>]` with an integration flag enabled, **the factory wins for that integration
point**. E.g. an `int`-keyed Smart Enum with `[ObjectFactory<string>(UseForSerialization = SystemTextJson)]`
serializes as the string, not the int. Priority is per integration point (serialization / EF /
model binding) independently. If serialization "ignores the key", look for a competing factory.

## Diagnostics

`TTRESG059` (missing matching ctor), `060` (Smart Enum + `HasCorrespondingConstructor`),
`061` (missing/bad `Validate`), `062` (missing `ToValue`), `068`/`069`/`070` (multi-factory
conflicts).

## Worked examples in this repo

Real, compiled examples (correct by construction — CI builds them):

- Samples: `samples/Basic.Samples/ValueObjects/FileUrn.cs`, `samples/Basic.Samples/Unions/TextOrNumberSerializable.cs`,
  `samples/EntityFrameworkCore.Samples/ValueConversion/ObjectFactoryValueConverter.cs`.
- Tests: `test/Thinktecture.Runtime.Extensions.Tests.Shared/TestEnums/` (the `SmartEnum_*ObjectFactory*.cs` variants).
- Docs: `docs/Object-Factories.md`.

## Exact details

For precise property types/defaults and the `SerializationFrameworks` enum, query **context7** for
`Thinktecture.Runtime.Extensions` or see https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Object-Factories. For the `SkipFactoryMethods`
cascade (which an `[ObjectFactory<T>]` can override), see `references/value-objects.md`.
