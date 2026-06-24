---
name: thinktecture-runtime-extensions
description: >-
  Design and implement domain types with Thinktecture.Runtime.Extensions — Smart Enums
  ([SmartEnum<TKey>] / [SmartEnum]), Value Objects ([ValueObject<TKey>] / [ComplexValueObject]),
  Discriminated Unions ([Union<T1,T2,…>] / [AdHocUnion] / [Union]), and Object Factories
  ([ObjectFactory<T>]). Use both to pick the right building block AND to implement it: validation via
  ValidateFactoryArguments / CreateCore / ValidateCore, Switch/Map, factory methods
  (FactoryMethodGeneration), equality comparers (KeyMemberEqualityComparer), generic types via
  TypeParamRef1-5, backing-field merging
  (UseSingleBackingField), serialization/EF/ASP.NET integration, setting cascades, and TTRESG
  diagnostics. Trigger on any of these attributes, primitive-obsession refactors (Email, Amount,
  ISBN), fixed enum-like sets, one-of / Success|Failure modeling, IsT1/AsT1, or TTRESG codes
  (047/057-062/068-076). Type-specific detail is loaded on demand from this skill's references/.
---

# Thinktecture.Runtime.Extensions

A .NET library that turns plain `partial` types into **Smart Enums**, **Value Objects**, and
**Discriminated Unions** via Roslyn source generators. You declare a type and an attribute; the
generator emits factory methods, equality, conversions, pattern matching, parsing, and serializer
integration. `[ObjectFactory<T>]` adds custom conversion to/from an arbitrary `T` — even on plain
classes.

This single skill covers all four families. Start here to decide *which* type fits and for the
rules shared across all of them, then **load the matching reference file for the concrete work.**

## Pick the type

| You want…                                                              | Attribute                                          | Reference file                       |
|------------------------------------------------------------------------|----------------------------------------------------|--------------------------------------|
| A type-safe enum that carries data/behavior, optional underlying key   | `[SmartEnum<TKey>]` / `[SmartEnum]`                | `references/smart-enums.md`          |
| A single-value or multi-property immutable domain primitive            | `[ValueObject<TKey>]` / `[ComplexValueObject]`     | `references/value-objects.md`        |
| A "one of N types" value (ad-hoc) or a closed inheritance hierarchy    | `[Union<T1,T2,…>]` / `[AdHocUnion(…)]` / `[Union]`  | `references/discriminated-unions.md` |
| Custom conversion to/from an arbitrary `T` — incl. on **plain classes**  | `[ObjectFactory<T>]`                              | `references/object-factories.md`     |

**Cross-cutting** references apply across families — load them in addition to the family file:

| You're working on…                                                     | Reference file                       |
|------------------------------------------------------------------------|--------------------------------------|
| `Switch`/`Map` pattern matching (Smart Enums **and** Unions)           | `references/switch-map.md`           |
| Serialization (JSON/MessagePack), ASP.NET model binding, Swashbuckle, EF Core, Serilog logging | `references/framework-integration.md` |
| A **generic** type (`Foo<T>`) in any family — the `TypeParamRef1`–`TypeParamRef5` placeholders | `references/generic-types.md` |
| String / case- / culture-specific **equality & ordering** comparers (Enums **and** VOs) | `references/equality-and-comparers.md` |
| A multi-family / non-obvious **combination** (task → which refs + which sample) | `references/recipes.md` |
| Looking up a **`TTRESG####` diagnostic** proactively | `references/diagnostics.md` |
| Library **utility helpers** (`TrimOrNullify`, `Empty.*`, `SingleItem.*`, `ToReadOnlyCollection`) | `references/utilities.md` |

Quick discriminators when unsure:

- **Smart Enum** — a *fixed, known set* of named instances (statuses, categories). Items are
  `public static readonly` fields.
- **Value Object** — an *open set* of values validated at creation (Email, Amount, ISBN). No fixed
  instances; created via `Create`/`TryCreate`. A lone wrapped/validated value (PostalCode, Email) is
  a Value Object — don't overthink it.
- **Union** — a value that is *exactly one of several shapes* (Success | Failure, Cat | Dog). Use
  ad-hoc for unrelated existing types, regular for a closed hierarchy you own.
- **Object Factory** — not a type family but an escape hatch: custom (de)serialization formats, model
  binding of non-string keys, single-column EF persistence, or giving a plain class one consistent
  representation.

## How to use this skill (progressive disclosure)

1. Use the table above to identify the family (or families).
2. **Read the matching `references/*.md` file before writing any family-specific code** — it carries
   the full design rules, generated surface, settings cascades, diagnostics, pitfalls, and worked
   examples for that family. Do not design or generate code for a family from memory; load its
   reference first.
3. **If the task spans more than one family, load EVERY relevant reference — not just the first.**
   Common multi-family combinations:
   - Serializing an **ad-hoc union** as a single value → `discriminated-unions.md` **and**
     `object-factories.md` (the union has no discriminator; the factory defines the representation).
   - A **keyless Smart Enum** that needs JSON / model binding → `smart-enums.md` **and**
     `object-factories.md` (a keyless enum has no key, so a factory is the *only* route).
   - Custom (de)serialization / single-column EF for a **Value Object** → `value-objects.md` **and**
     `object-factories.md`.
   Stopping after the first reference is the main failure mode here — if serialization, model
   binding, or EF is mentioned for a keyless/ad-hoc type, you almost certainly also need
   `object-factories.md`.
   For the **registration mechanics** of any of the above (which package to add, or which converter /
   resolver / model-binder / EF extension method to call), also load `references/framework-integration.md`.
   For `Switch`/`Map` on Smart Enums or unions, load `references/switch-map.md`. If the type is
   **generic** (`Foo<T>`), load `references/generic-types.md`; for string/custom comparers,
   `references/equality-and-comparers.md`. For non-obvious multi-family combinations, `references/recipes.md`
   is a task→sample index.
4. For exact attribute property names, defaults, and method signatures, query the **context7 MCP**
   for `Thinktecture.Runtime.Extensions` rather than guessing (the surface is large and
   version-specific). If context7 is unavailable, see the wiki at
   https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki.

## Cross-cutting rules (apply to all families)

- **Every type must be `partial`.** The generator emits the other half. Missing `partial` is the
  most common first error.
- **Don't hand-write what's generated.** Factory methods (`Create`/`TryCreate`/`Validate`),
  equality (`Equals`/`GetHashCode`/`==`/`!=`), conversions, and `Switch`/`Map` come from the
  generator. Writing them yourself causes conflicts or is silently ignored.
- **The analyzer guides you.** `TTRESG#####` diagnostics report misuse with precise fixes; many
  have code fixes. Treat an analyzer error as the authoritative instruction, not noise. The full
  table is in https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Analyzer-Diagnostics;
  each reference file lists the codes relevant to it.
- **Validation hook:** prefer `ValidateFactoryArguments` (runs only via factory methods) over
  `ValidateConstructorArguments` for Value Objects. Details in `references/value-objects.md`.
- **Serialization/EF/ASP.NET/Serilog are opt-in via integration packages** (`*.Json`, `*.Newtonsoft`,
  `*.MessagePack`, `*.EntityFrameworkCore8/9/10`, `*.AspNetCore`, `*.Swashbuckle`, `*.Serilog`). Add the package
  and integration is auto-generated; no manual converter wiring. **Verify the relevant integration
  package is referenced** before relying on auto-generated converters — a missing package is the
  usual cause of "I added the attribute but serialization didn't happen." Full registration mechanics
  (package vs manual converter/resolver/provider, EF extension methods, Serilog destructuring) are in
  `references/framework-integration.md`.
- **`[ObjectFactory<T>]` is the escape hatch for custom conversion** — and it works even on a plain
  `partial class`, not just generated types. See `references/object-factories.md`.

## Settings that cascade (read before configuring)

Several attribute settings silently force others (e.g. `SkipFactoryMethods` also disables parsing
and serializer converters). Never describe a setting in isolation — account for the cascade. The
per-family cascade tables live in the reference files (*Setting cascade* in `references/value-objects.md`,
*Setting interactions* in `references/discriminated-unions.md`).

Project-level MSBuild options (source-generator logging, disabling JetBrains annotations) are rarely
needed — see https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Source-Generator-Configuration
if required.

## Upgrading versions

When the task is migrating between major versions (breaking changes in attributes, generated surface,
or settings), consult the migration guides rather than guessing — the most recent is
https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Migration-from-v9-to-v10 (earlier:
v6→v7, v7→v8, v8→v9). They list renamed/removed settings and behavioral changes that this skill's
"current state" guidance does not call out.
