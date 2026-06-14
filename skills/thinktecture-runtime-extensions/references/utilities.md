# Utility Helpers

Small, allocation-conscious helpers shipped in the core package (namespace `Thinktecture`). They belong
to no type family but show up often **inside** Smart Enum / Value Object work — especially
`TrimOrNullify` for normalization in a validation hook. Reach for these instead of hand-rolling the
equivalent.

## `TrimOrNullify` — normalize a string in one call

`string?` extension. Returns `null` if the input is `null`, empty, or whitespace-only; otherwise trims,
and (with `maxLength`) truncates.

```csharp
var trimmed          = value.TrimOrNullify();              // null when blank, else trimmed
var trimmedShortened = value.TrimOrNullify(maxLength: 50); // also capped to 50 chars
```

Idiomatic in `ValidateFactoryArguments`, where the parameter is `ref` so the normalized value flows to
equality, serialization, and persistence alike:

```csharp
static partial void ValidateFactoryArguments(ref ValidationError? validationError, ref string value)
{
    value = value.TrimOrNullify()!;                 // collapse blank/padding first
    if (value is null)
        validationError = new ValidationError("Value cannot be empty");
}
```

## `Empty.*` — empty delegates & read-only collections

Cached, no-op / empty instances so you don't allocate a fresh `new List<T>()` or `() => {}` for a
default or fallback:

```csharp
MyMethod(Empty.Action);                              // no-op Action / Action<T> (overloaded)
IDisposable d       = Empty.Disposable();            // also Empty.AsyncDisposable()
IReadOnlyList<int>  l = Empty.Collection<int>();     // also Empty.Collection() → IEnumerable
IReadOnlyDictionary<string,int> map = Empty.Dictionary<string, int>();
ILookup<string,int> lk = Empty.Lookup<string, int>();
IReadOnlySet<string> s = Empty.Set<string>();
```

## `SingleItem.*` — one-element read-only collections

Resource-saving wrappers for a single value, e.g. to let a single-item overload delegate to a
collection overload without building a full collection:

```csharp
IReadOnlySet<string>            set  = SingleItem.Set("name");
IReadOnlyDictionary<int,string> dict = SingleItem.Dictionary(42, "name");
ILookup<int,string>             lk   = SingleItem.Lookup(42, new[] { "a", "b" });
```

## `ToReadOnlyCollection` — project / wrap without copying

Avoids the `.Select(...).ToList()` copy when an API wants `IReadOnlyCollection<T>`:

```csharp
IReadOnlyCollection<string> names = users.ToReadOnlyCollection(u => u.Name);          // projecting overload
IReadOnlyCollection<string> also  = users.Select(u => u.Name).ToReadOnlyCollection(users.Count); // count overload
```

## Worked examples in this repo

- Docs: `docs/TrimOrNullify.md`, `docs/Empty-....md`, `docs/SingleItem.md`, `docs/ToReadOnlyCollection.md`.

## Exact details

For precise overload signatures, query **context7** for `Thinktecture.Runtime.Extensions`.
