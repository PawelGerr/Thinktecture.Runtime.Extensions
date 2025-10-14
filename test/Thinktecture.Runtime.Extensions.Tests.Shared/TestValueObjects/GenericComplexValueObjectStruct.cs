using System;

namespace Thinktecture.Runtime.Tests.TestValueObjects;

[ComplexValueObject]
public partial struct GenericComplexValueObjectStruct<TClass, TStruct, T>
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
