using System.Diagnostics.CodeAnalysis;

namespace Thinktecture;

public delegate bool SelectWhereDelegate<in T, TResult>(T item, [MaybeNullWhen(false)] out TResult result);

public delegate bool SelectWhereDelegate<in T, in TArg, TResult>(T item, TArg arg, [MaybeNullWhen(false)] out TResult result);
