# Framework Integration

How generated types plug into **JSON** (System.Text.Json, Newtonsoft.Json), **MessagePack**,
**ASP.NET Core** (model binding), **Swashbuckle/OpenAPI**, and **EF Core**. This file collects the
registration mechanics shared by all families; the per-family reference files
(`smart-enums.md`, `value-objects.md`, `discriminated-unions.md`) only point here. For *custom*
representations (non-key formats, single-column persistence of multi-member types, keyless/ad-hoc
types) see `references/object-factories.md` — this file covers the **built-in, key-based** path.

## The two activation models

For every serializer there are two ways to turn integration on:

1. **Reference the integration package** from the project that *contains* the types (direct or
   transitive). This activates extra code generation that flags each type with the serializer's
   attribute (`[JsonConverter]`, `[MessagePackFormatter]`, …) — **zero wiring**. This is the simplest
   and recommended path.
2. **Register a converter/resolver/provider** with the serializer's settings. Use when you can't or
   don't want to add the package to the type's project — install it wherever the settings are
   configured instead.

**The usual cause of "I added the attribute but (de)serialization didn't happen" is a missing
integration package** (model 1) *and* no manual registration (model 2). Verify one of the two is in
place before debugging further.

> **Object factories are different:** the auto-activation above applies to the **key-based** path.
> `[ObjectFactory<T>]` integration is **never** turned on by a package reference alone — you must set
> its flags explicitly (`UseForSerialization`, `UseForModelBinding`, `UseWithEntityFramework`; all
> default off). A keyless enum or ad-hoc union with a factory but no flags set serializes/binds
> nowhere. See `object-factories.md`.

| Concern | Package | Manual registration (model 2) |
|---|---|---|
| System.Text.Json | `Thinktecture.Runtime.Extensions.Json` | `ThinktectureJsonConverterFactory` → `JsonSerializerOptions.Converters` |
| Newtonsoft.Json | `Thinktecture.Runtime.Extensions.Newtonsoft.Json` | `ThinktectureNewtonsoftJsonConverterFactory` → `JsonSerializerSettings.Converters` |
| MessagePack | `Thinktecture.Runtime.Extensions.MessagePack` | `ThinktectureMessageFormatterResolver.Instance` in a `CompositeResolver` |
| ASP.NET Core | `Thinktecture.Runtime.Extensions.AspNetCore` | `ThinktectureModelBinderProvider` → MVC `ModelBinderProviders` |
| OpenAPI | `Thinktecture.Runtime.Extensions.Swashbuckle` | `AddThinktectureOpenApiFilters()` on the service collection |
| EF Core | `Thinktecture.Runtime.Extensions.EntityFrameworkCore8/9/10` | `UseThinktectureValueConverters()` / `AddThinktectureValueConverters()` |

## JSON

```csharp
// System.Text.Json — minimal API
builder.Services.ConfigureHttpJsonOptions(o =>
    o.SerializerOptions.Converters.Add(new ThinktectureJsonConverterFactory()));

// System.Text.Json — MVC
services.AddMvc().AddJsonOptions(o =>
    o.JsonSerializerOptions.Converters.Add(new ThinktectureJsonConverterFactory()));

// Newtonsoft.Json — MVC
services.AddMvc().AddNewtonsoftJson(o =>
    o.SerializerSettings.Converters.Add(new ThinktectureNewtonsoftJsonConverterFactory()));
```

- **Keyed Smart Enums / simple Value Objects** serialize as their key automatically once a package is
  referenced (or a factory registered) — no `[ObjectFactory<T>]` needed.
- **Zero-allocation JSON (NET9+, string keys, System.Text.Json only):** automatic; the generated
  converter and `ThinktectureJsonConverterFactory` both pick the span-based path. Opt out per type
  with `DisableSpanBasedJsonConversion = true`, or at runtime via the factory's
  `skipSpanBasedDeserialization` predicate.
- **Keyless Smart Enums and ad-hoc unions have no key/discriminator** → JSON needs
  `[ObjectFactory<T>]` (see `object-factories.md`). Regular unions serialize polymorphically via
  `[JsonDerivedType]` on the base (System.Text.Json) or `TypeNameHandling` (Newtonsoft.Json, no
  integration package needed).

## MessagePack

```csharp
var resolver = CompositeResolver.Create(
    ThinktectureMessageFormatterResolver.Instance, StandardResolver.Instance);
var options = MessagePackSerializerOptions.Standard.WithResolver(resolver);
```

Referencing the package instead flags types with `[MessagePackFormatter]` automatically.

## ASP.NET Core model binding

- **Minimal APIs**: a type is bindable from route/query if it implements `IParsable<T>` (`TryParse`).
  Keyed Smart Enums and simple Value Objects get this automatically when the key is `string` or
  implements `IParsable<TKey>` — **no setup**. Limitation: minimal-API binding can't surface a
  custom `ValidationError`; a failed bind is a generic `400`. To return a *specific* error instead,
  wrap the parameter in a `MaybeBound<T, TKey, TValidationError>` (a small **app-side** helper, not
  generated): its `TryParse` always succeeds, capturing either the value or the error, and an endpoint
  filter — or, on .NET 10, `IValidatableObject` — turns the captured error into the response. Sample:
  `samples/AspNetCore.Samples/Validation/MaybeBound.cs` (used in `samples/AspNetCore.Samples/Program.cs`).
- **MVC controllers**: register the model binder provider, inserted **before** the defaults so they
  don't try to bind these types first:

  ```csharp
  services.AddControllers(o =>
      o.ModelBinderProviders.Insert(0, new ThinktectureModelBinderProvider()));
  ```

  `ThinktectureModelBinderProvider(skipBindingFromBody: true)` (the default) leaves request-body
  values to the JSON serializer.
- **Non-string keys, keyless enums, complex Value Objects, unions**: add
  `[ObjectFactory<string>(UseForModelBinding = true)]` so route/query strings bind — the attribute
  alone is **not** enough; you still register `ThinktectureModelBinderProvider` for MVC. See
  `object-factories.md`.

## OpenAPI / Swashbuckle

One registration covers Smart Enums and Value Objects:

```csharp
services.AddEndpointsApiExplorer()
        .AddSwaggerGen(o => o.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" }))
        .AddThinktectureOpenApiFilters(o =>
        {
            o.SmartEnumSchemaFilter    = SmartEnumSchemaFilter.Default;                       // enum: [..]
            o.SmartEnumSchemaExtension = SmartEnumSchemaExtension.VarNamesFromStringRepresentation;
            o.RequiredMemberEvaluator  = RequiredMemberEvaluator.Default;                      // VO required-ness
        });
```

- `SmartEnumSchemaFilter`: `Default` (`enum: [...]`), `OneOf`, `AnyOf`, `AllOf`,
  `FromDependencyInjection`. `AllOf` appends one combined `enum` subschema to `allOf` (a per-item
  `const` conjunction would be unsatisfiable); pre-existing `allOf` entries such as a `$ref` are
  preserved.
- `SmartEnumSchemaExtension`: `None`, `VarNamesFromStringRepresentation`,
  `VarNamesFromDotnetIdentifiers`, `FromDependencyInjection` (controls `x-enum-varnames`).
- `RequiredMemberEvaluator` (Value Objects): `Default` (struct VO without `AllowDefaultStructs` or a
  non-nullable reference is required), `All`, `None`, `FromDependencyInjection`. `[Required]` still
  works for per-property overrides.

Output: simple VOs / keyed enums render as their underlying type; complex VOs render as objects with
their properties.

## EF Core

Pick the package matching your EF Core major version: `…EntityFrameworkCore8`, `…9`, or `…10`. Then
register value converters at the broadest scope that fits. Registration levels (broadest → narrowest):

| Level | Extension method | Use when |
|---|---|---|
| `DbContextOptionsBuilder` *(recommended)* | `UseThinktectureValueConverters()` | Global, no `OnModelCreating` changes |
| `ModelBuilder` | `AddThinktectureValueConverters()` | All Smart Enums & Value Objects in the model |
| `EntityTypeBuilder` | `AddThinktectureValueConverters()` | One entity / owned / complex type |
| `PropertyBuilder` | `HasThinktectureValueConverter()` | One property |

```csharp
services.AddDbContext<AppDbContext>(b => b
    .UseSqlServer(connectionString)
    .UseThinktectureValueConverters());   // recommended: one call, whole context
```

- All levels except fully-manual converters accept an optional `Configuration` (max-length strategy
  etc.); the parameterless overloads use `Configuration.Default`, which auto-computes max length for
  string-keyed Smart Enums. No max-length strategy is applied to a type with an
  `[ObjectFactory<T>(UseWithEntityFramework = true)]`, because the persisted value comes from the
  factory and may differ from the key; use `HasMaxLength` explicitly for such types.
- **Owned entities, complex types, and primitive collections** have their own overloads when the
  VOs/enums don't sit directly on the entity: `AddThinktectureValueConverters` on an
  `OwnedNavigationBuilder` or `ComplexPropertyBuilder`, and `HasThinktectureValueConverter` on a
  `ComplexTypePropertyBuilder` or `PrimitiveCollectionBuilder` (e.g. a `List<MyValueObject>` column).
- **Custom / single-column persistence of complex VOs, keyless enums, ad-hoc unions** → use
  `[ObjectFactory<T>(UseWithEntityFramework = true)]` (requires `ToValue()`); see `object-factories.md`.
  `HasCorrespondingConstructor = true` lets EF **bypass validation on DB read** (DB is the source of
  truth).
- **Regular unions** can use standard EF inheritance (TPH/TPT); a factory is the single-column
  alternative.

## Choosing which frameworks generate code

Limit key-based converter generation with `SerializationFrameworks` on the type's attribute (e.g.
`[SmartEnum<string>(SerializationFrameworks = SerializationFrameworks.SystemTextJson)]`). Values:
`None`, `SystemTextJson`, `NewtonsoftJson`, `Json` (both JSONs), `MessagePack`, `All` (default);
combine with `|`. On `[ObjectFactory<T>]`, `UseForSerialization` plays the same role for the custom
path and **takes priority over key-based conversion** when both exist (per integration point — see
the metadata-priority note in `object-factories.md`).

## Serilog

Package: `Thinktecture.Runtime.Extensions.Serilog` (requires Serilog `>= 4.0.0`). Register once at startup:

```csharp
using Thinktecture;

Log.Logger = new LoggerConfiguration()
    .Destructure.UsingThinktectureRuntimeExtensions()
    .WriteTo.Console()
    .CreateLogger();
```

**Behavior by type:**

| Type | Logged as |
|---|---|
| Keyed Smart Enum | Underlying key (`OrderStatus.Paid` → `"Paid"`) |
| Simple Value Object | Underlying key (`Amount.Create(99.95m)` → `99.95`) |
| Ad-hoc Union | Current `Value` (recurses through nested Thinktecture types) |
| Keyless Smart Enum | Serilog default (not handled) |
| Complex Value Object | Serilog default (not handled) |
| Regular Union (`[Union]`) | Serilog default (not handled) |

Object factories are **always ignored** for logging — the key is used regardless of any `[ObjectFactory<T>]` attribute.

**`renderAsString` flag**: optionally coerce matched types to log as string via `ToString()` instead of as a scalar:

```csharp
.Destructure.UsingThinktectureRuntimeExtensions(
    renderAsString: TypesToRenderAsString.SmartEnums | TypesToRenderAsString.ValueObjects)
```

Available flags: `None` (default), `SmartEnums`, `ValueObjects`, `AdHocUnions`, `All`.

**Caveats:**
- `default(struct union)` throws `InvalidOperationException` when its `Value` is read — ensure struct unions are initialized before logging.
- Types with `SkipToString = true` log as their **type name** (not key) when opted into `renderAsString`.
- Use `{@Property}` (the destructuring operator) — without `@` Serilog calls `ToString()` directly.
- Recursion uses `destructureObjects: true`, so a non-Thinktecture inner value (e.g. a union wrapping a plain POCO) is reflected by Serilog. Bound large graphs with Serilog's native `Destructure.ToMaximumDepth`/`ToMaximumCollectionCount`/`ToMaximumStringLength` — there is no Thinktecture-specific setting.

## Exact details

For precise method/option names and per-version differences, query **context7** for
`Thinktecture.Runtime.Extensions`, or see the wiki: https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Smart-Enums-Framework-Integration,
https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Value-Objects-Framework-Integration, and
https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Discriminated-Unions-Framework-Integration.

For Serilog integration details, see https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Serilog.

For custom conversion formats see `references/object-factories.md`.
