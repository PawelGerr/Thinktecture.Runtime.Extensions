// TODO: Uncomment after TypeParamRef support is implemented for SmartEnum (see typeparamref-for-smartenum-valueobject.md)
// Currently blocked by CS8968: C# prohibits type parameters as attribute type arguments.
// The TypeParamRef placeholder pattern (already used by unions) must be extended to SmartEnum first.

// namespace Thinktecture.Runtime.Tests.TestEnums;
//
// // ReSharper disable once InconsistentNaming
// [SmartEnum<TypeParamRef1>]
// public partial class SmartEnum_GenericKeyBased<T>
// {
//    public static readonly SmartEnum_GenericKeyBased<T> Item1 = default!;
//    public static readonly SmartEnum_GenericKeyBased<T> Item2 = default!;
// }
