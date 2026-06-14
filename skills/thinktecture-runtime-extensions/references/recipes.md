# Recipes — cross-family combinations

Task-oriented index for the **non-obvious combinations** that span more than one reference file — the
cases where loading a single family file isn't enough. Each row points at the references to load and a
**real, compiled** sample/test that demonstrates the combination end to end (CI builds them, so they
can't drift). For single-family work, start from the family file instead.

| I want to… | Load | Real example |
|---|---|---|
| Serialize an **ad-hoc union** as a single value (JSON/MessagePack/EF) — it has no discriminator | `discriminated-unions.md` + `object-factories.md` + `framework-integration.md` | `samples/Basic.Samples/Unions/TextOrNumberSerializable.cs` |
| Serialize / model-bind / EF-persist a **keyless Smart Enum** — it has no key to convert from | `smart-enums.md` + `object-factories.md` + `framework-integration.md` | `test/Thinktecture.Runtime.Extensions.Tests.Shared/TestEnums/SmartEnum_Keyless_ObjectFactory.cs` |
| Give a **Value Object** a custom (de)serialization format or single-column EF persistence | `value-objects.md` + `object-factories.md` | `samples/Basic.Samples/ValueObjects/FileUrn.cs` |
| Flow extra **context into validation** (e.g. a rounding strategy) via `CreateCore`/`ValidateCore` | `value-objects.md` | `samples/Basic.Samples/ValueObjects/Money.cs`, `…/BoundaryWithFactories.cs` |
| Put one consistent representation on a **plain `partial class`** (no enum/VO/union attribute) | `object-factories.md` | the `CustomObject` plain-class example in `object-factories.md`, `docs/Object-Factories.md` |
| A **Smart Enum that inherits a base class** (per-item data on the base) | `smart-enums.md` | `samples/Basic.Samples/SmartEnums/EnumWithBaseClass.cs` |
| A **generic** Smart Enum / Value Object / ad-hoc union (`Foo<T>`) | `generic-types.md` (+ the family file) | `test/Thinktecture.Runtime.Extensions.Tests.Shared/TestAdHocUnions/TestUnion_generic_struct_constrained_TypeParamRef1_string.cs` |
| Case- or culture-specific **string equality** on an enum/VO | `equality-and-comparers.md` | `samples/Basic.Samples/SmartEnums/ProductCategoryWithCaseSensitiveComparer.cs` |
| **Zero-allocation JSON** (NET9+) for a custom format | `object-factories.md` | `docs/Object-Factories.md` (*Zero-Allocation JSON*) |

## The recurring trap

When serialization, model binding, or EF is mentioned for a **keyless Smart Enum** or an **ad-hoc
union**, you almost always need `object-factories.md` too — those types have no key/discriminator, so a
factory is the *only* route, and a package reference alone never activates it (its flags default off).
Stopping after the first family file is the main failure mode. See the note at the top of
`references/framework-integration.md`.

## Broader sample/test inventory

Each family reference file lists its own samples and the exhaustive test directories under
`test/Thinktecture.Runtime.Extensions.Tests.Shared/` (`TestEnums/`, `TestValueObjects/`,
`TestAdHocUnions/`, `TestRegularUnions/`). Prefer copying from those over generating from memory.
