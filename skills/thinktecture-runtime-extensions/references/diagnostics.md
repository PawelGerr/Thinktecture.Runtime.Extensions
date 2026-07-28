# Analyzer Diagnostics (curated)

The library ships ~60 `TTRESG####` rules across two analyzers
(`ThinktectureRuntimeExtensionsAnalyzer` for usage, `…InternalUsageAnalyzer` for internal-API misuse).
**Treat an analyzer error as the authoritative instruction** — many have code fixes. The full,
authoritative table is `docs/Analyzer-Diagnostics.md` (and the wiki). This file is a curated subset of
the codes worth knowing **proactively** because they prevent generating non-compiling or silently-wrong
code; check the full table for anything not here.

## General / shared (all families)

| Code | Meaning |
|---|---|
| TTRESG006 | Type must be `partial` — **every enclosing type too**. The #1 "nothing generated" cause. |
| TTRESG001 / TTRESG003 | Field must be `readonly` / property must be get-only (and TTRESG034/035 for base-class members, TTRESG042 for `init`). |
| TTRESG004 | The type must be a `class` or a `struct`. |
| TTRESG009 | All constructors must be `private`. |
| TTRESG017 | The key member must not be nullable. |
| TTRESG043 | Primary constructors are not allowed — use a regular `private` constructor. |
| TTRESG046 | `Switch`/`Map` arguments must be **named** (see `references/switch-map.md`). |
| TTRESG047 | A value-object/enum variable must be created via a factory, not `default`. |
| TTRESG063 | Only one category attribute (`SmartEnum`/`ValueObject`/`Union`) per type. |
| TTRESG064 / TTRESG065 / TTRESG066 | More than one `SmartEnum` / `ValueObject` / `Union` attribute on a type (the per-family duplicates of TTRESG063). |

## Smart Enum

| Code | Meaning |
|---|---|
| TTRESG002 | Smart Enum item field must be `public`. |
| TTRESG014 / TTRESG015 | Inner derived enum: first level must be `private`, deeper levels `public`. |
| TTRESG036 | The key type argument must not be nullable. |
| TTRESG037 | A Smart Enum with no derived types must be `sealed` (or add derived inner types). |
| TTRESG100 / TTRESG101 | No items (warning) / static **property** isn't an item — use a `static readonly` **field**. |

## Value Object

| Code | Meaning |
|---|---|
| TTRESG044 / TTRESG045 | Custom key member not found / key member type mismatch (with `SkipKeyMember`). |
| TTRESG048 / TTRESG049 | String VO needs an equality comparer / complex VO with string members needs a comparison (see `references/equality-and-comparers.md`). |
| TTRESG057 / TTRESG058 | `AllowDefaultStructs` must be `false` when the key is a reference type / a member disallows defaults. |
| TTRESG041 | Comparer's generic argument doesn't match the member type. |
| TTRESG102 / TTRESG103 | Comparer without equality comparer / equality comparer without comparer. |
| TTRESG104 | A member should be `required` to ensure initialization. |
| TTRESG105 | `ComparisonOperators` and `EqualityComparisonOperators` settings mismatch. |

## Discriminated Union

| Code | Meaning |
|---|---|
| TTRESG053 | A derived union type must not be generic. |
| TTRESG054 | A regular union must be `sealed` or have only private constructors (non-sealed intermediates need a private ctor). |
| TTRESG055 | A **record**-based union must be `sealed` — records can't nest; use classes for nested hierarchies. |
| TTRESG056 | A non-abstract derived union must not be less accessible than the base. |
| TTRESG067 | An ad-hoc union must declare at least two member types. |
| TTRESG071 / TTRESG072 | `TypeParamRef` index out of range / used on a non-generic union. |
| TTRESG073 | Ad-hoc unions don't support `allows ref struct` type parameters. |
| TTRESG074 | A `TypeParamRef` used as a key needs `where T : notnull`. |
| TTRESG075 | `SingleBackingFieldType` conflicts with `UseSingleBackingField = false`. |
| TTRESG076 | An extra `ValidateFactoryArguments` parameter must be by-value with no default (no `ref`/`out`/default). |
| TTRESG077 | An ad-hoc union member type must not be less accessible than the union (the generated operators would not compile). |
| TTRESG106 / TTRESG107 | Inner type should derive from the union / generic ad-hoc union references no type parameter via `TypeParamRef`. |

## Object Factory

| Code | Meaning |
|---|---|
| TTRESG050 / TTRESG051 | `[UseDelegateFromConstructor]` method must be `partial` / must not be generic. |
| TTRESG059 / TTRESG060 | `HasCorrespondingConstructor = true` needs a matching single-arg ctor / not allowed on Smart Enums. |
| TTRESG061 / TTRESG062 | Missing/mis-signed static `Validate` / missing `ToValue()` when writing is enabled. |
| TTRESG068 / TTRESG069 / TTRESG070 | Multiple factories conflict on EF / model binding / overlapping serialization frameworks. |

## Internal-API & info

- **TTRESG1000** — using a `Thinktecture.Internal` type from outside (different category:
  `ThinktectureRuntimeExtensionsInternalUsageAnalyzer`).
- **TTRESG1001** — consider a `static` lambda for `Switch`/`Map` (info; see `references/switch-map.md`).
- **TTRESG097–099** — internal generator failures; report if persistent (097/099 are errors, **098 is a warning**).

## Suppressing diagnostics

Rarely correct for errors (they signal broken generation). When you must, scope it tightly:

```csharp
#pragma warning disable TTRESG048
[ValueObject<string>] public partial class MyVo;
#pragma warning restore TTRESG048

[SuppressMessage("ThinktectureRuntimeExtensionsAnalyzer", "TTRESG048")]   // TTRESG1000 uses the *InternalUsageAnalyzer* category
```

`.editorconfig`: `dotnet_diagnostic.TTRESG048.severity = none`.

## Exact details

Full table with severities and per-rule guidance: `docs/Analyzer-Diagnostics.md` and
https://github.com/PawelGerr/Thinktecture.Runtime.Extensions/wiki/Analyzer-Diagnostics.
