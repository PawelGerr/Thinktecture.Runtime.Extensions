# Switch / Map

Generated, **exhaustive** pattern matching emitted for **Smart Enums** (keyed and keyless) and
**Discriminated Unions** (ad-hoc and regular). This is the call-site counterpart to the design rule
"don't branch on `this`": instead of `if`/`switch` over items or shapes, you call generated `Switch`
/ `Map` and the compiler forces you to handle every case.

This file is shared by `references/smart-enums.md` and `references/discriminated-unions.md` — read it
once for either family.

## Switch vs Map

- **`Switch(...)`** — arms are **lambdas**. Two overloads: a `void` form (each arm is an action, for
  side effects) and a value-returning form (each arm is a `Func<TResult>` that *computes* the
  result). Use `Switch` whenever an arm needs logic.
- **`Map(...)`** — arms are **pre-computed values**, not lambdas. Use it only when each result is a
  constant already in hand.

```csharp
// Switch (void) — side effects
animal.Switch(dog: d => Log(d.Name), cat: c => Log(c.Name));

// Switch (value-returning) — each arm is a lambda that computes the result.
// Explicit <IActionResult> unifies the differing arm return types (see "Value-returning arms" below).
var action = result.Switch<IActionResult>(
    success:  static _ => new OkResult(),
    notFound: static _ => new NotFoundResult(),
    failure:  static f => new BadRequestObjectResult(f.Reason));

// Map — each arm is a constant
var label = animal.Map(dog: "Dog", cat: "Cat");
```

The value-returning `Switch` is the usual way to turn a union/enum into a domain value (an
`IActionResult`, a DTO, …) when the arms need expressions rather than constants — reach for `Map`
only when every arm is a literal value.

Arm names come from the member/case names (Smart Enum item names, union member names — adjustable via
`T1Name`/`T2Name`… on ad-hoc unions, or the case type names on regular unions).

## Why prefer it over native `switch`

A native `switch`/`switch` expression needs a `_ => …` discard arm, which **silently swallows newly
added cases** — add a Smart Enum item or union case and existing call sites keep compiling against
stale logic. Generated `Switch`/`Map` are exhaustive: every case is a required argument, so adding a
case becomes a **compile error at every call site** until you handle it. That compiler pressure is
the main reason to use them.

## Partial matching: `SwitchPartially` / `MapPartially`

When you genuinely want to handle a subset, use `SwitchPartially` / `MapPartially`, which take a
fallback (`@default:`) for the unhandled cases. Reach for these deliberately — they reintroduce the
"silent fallthrough" you avoided above, so prefer the exhaustive form unless a fallback is the point.

For **nested regular unions** (e.g. `ApiResponse > Failure > NotFound | Unauthorized`), generate
additional non-exhaustive overloads with `[UnionSwitchMapOverload]`.

## Performance: pass state + static lambdas

Closing over outer variables allocates a closure per call. In hot paths, pass the captured state as
the **first argument** and use **static lambdas** — the state is forwarded to each arm, so nothing is
captured:

```csharp
u.Switch(logger,
         @string: static (l, s) => l.LogInformation("{S}", s),
         int32:   static (l, i) => l.LogInformation("{I}", i));
```

The same state-first overload exists for `Map`, `SwitchPartially`, and `MapPartially`.

## Value-returning arms with different types

If a `Map`/`Switch` that returns a value won't compile because the arms return *different but
compatible* types (e.g. two `List<int>` subtypes, or an interface and an implementation), specify the
result type explicitly:

```csharp
var items = u.Switch<IReadOnlyList<int>>(/* arms */);
```

## IDE assist: Switch/Map completion

There is a refactoring (light-bulb / quick action, `SwitchMapCompletionRefactoringProvider`) on any
`Switch` / `Map` / `SwitchPartially` / `MapPartially` call that **auto-fills all arms** for the type.
Type the call, invoke the light-bulb, and let it generate the argument list — this works in both
Rider and Visual Studio. After adding a new item/case, re-invoking it fills in the missing arm.

## Common pitfalls

- Falling back to native `switch` with `_ =>` and losing exhaustiveness when a case is added later.
- Using `SwitchPartially`/`MapPartially` (with a fallback) where the exhaustive form was intended.
- Allocating closures in hot paths instead of passing state + `static` lambdas.
- A value-returning `Map`/`Switch` that won't infer a common arm type — specify `TResult` explicitly.

## Worked examples in this repo

Real, compiled examples (correct by construction — CI builds them):

- Smart Enums: `samples/Basic.Samples/SmartEnums/SmartEnumDemos.cs`.
- Unions: `samples/Basic.Samples/Unions/DiscriminatedUnionsDemos.cs` and the type definitions in
  `samples/Basic.Samples/Unions/`.
- Docs: `docs/Smart-Enums.md`, `docs/Discriminated-Unions.md`.

For type-specific detail see `references/smart-enums.md` and `references/discriminated-unions.md`.
