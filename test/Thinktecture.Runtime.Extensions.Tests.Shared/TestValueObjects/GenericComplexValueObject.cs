using System;

namespace Thinktecture.Runtime.Tests.TestValueObjects;

[ComplexValueObject]
public partial class GenericComplexValueObject<TClass, TStruct, T>
   where TClass : class, IEquatable<TClass>
   where TStruct : struct
{
   public TClass ClassProperty { get; }
   public TClass? NullableClassProperty { get; }
   public TStruct StructProperty { get; }
   public TStruct? NullableStructProperty { get; }
   public T Property { get; }
   public T? NullableProperty { get; }
}

// Smoke test: same name but less generics
[ComplexValueObject]
public partial class GenericComplexValueObject<TClass, TStruct>
   where TClass : class, IEquatable<TClass>
   where TStruct : struct
{
   public TClass ClassProperty { get; }
   public TClass? NullableClassProperty { get; }
   public TStruct StructProperty { get; }
   public TStruct? NullableStructProperty { get; }
}

// Smoke test: same name but not generics
[ComplexValueObject(DefaultStringComparison = StringComparison.OrdinalIgnoreCase)]
public partial class GenericComplexValueObject
{
   public string ClassProperty { get; }
   public string? NullableClassProperty { get; }
   public int StructProperty { get; }
   public int? NullableStructProperty { get; }
   public bool Property { get; }
   public bool? NullableProperty { get; }
}
