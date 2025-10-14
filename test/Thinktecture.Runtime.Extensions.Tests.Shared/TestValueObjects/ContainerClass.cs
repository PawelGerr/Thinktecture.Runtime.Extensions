using System;

namespace Thinktecture.Runtime.Tests.TestValueObjects;

public partial class ContainerClass
{
   [ComplexValueObject(SerializationFrameworks = SerializationFrameworks.Json)]
   public partial class NestedGenericComplexValueObject<TClass, TStruct, T>
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

   [ComplexValueObject(SerializationFrameworks = SerializationFrameworks.SystemTextJson)]
   public partial struct NestedGenericComplexValueObjectStruct<TClass, TStruct, T>
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
}
