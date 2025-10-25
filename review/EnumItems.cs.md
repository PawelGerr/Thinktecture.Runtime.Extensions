- Warning: Hash code depends on EnumItem.GetHashCode(), which currently uses string.GetHashCode(Name). For explicit, culture-agnostic behavior and cross-process stability, prefer StringComparer.Ordinal.GetHashCode(Name) in EnumItem, or compute the collection hash with a comparer that uses Ordinal semantics for the underlying string.
  Suggested fix (in EnumItem):
  public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Name);
  Alternative (here): create an IEqualityComparer<EnumItem> that hashes via StringComparer.Ordinal on Name and use the comparer overload of ComputeHashCode.

- Warning: Constructor accepts ImmutableArray<IFieldSymbol> items without normalizing default arrays. If items.IsDefault is true, ImmutableArray.CreateRange(items, ...) may throw or lead to unexpected behavior. Normalize to Empty to avoid default-state pitfalls.
  Suggested fix:
  var normalized = items.IsDefault ? ImmutableArray<IFieldSymbol>.Empty : items;
  _items = ImmutableArray.CreateRange(normalized, i => new EnumItem(i));

- Nit: Missing explicit usings for System.Collections.Immutable, System.Linq, and Microsoft.CodeAnalysis. The file relies on global usings for ImmutableArray, SequenceEqual, and IFieldSymbol. Consider adding explicit using directives to avoid hidden dependencies on global usings.

- Nit: Readonly struct field _items can be left in IsDefault state for default(EnumItems). While most operations like Length return 0, avoiding IsDefault reduces surprises. Consider initializing the field to ImmutableArray<EnumItem>.Empty or providing a parameterless ctor to set it to Empty (C# 10+).
  Example:
  private readonly ImmutableArray<EnumItem> _items = ImmutableArray<EnumItem>.Empty;
