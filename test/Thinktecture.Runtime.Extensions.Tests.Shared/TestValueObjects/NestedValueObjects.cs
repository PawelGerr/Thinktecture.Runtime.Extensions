namespace Thinktecture.Runtime.Tests.TestValueObjects;

public partial class ParentClass
{
   [ValueObject<int>]
   public sealed partial class NestedClassTestValueObject;

   [ValueObject<int>]
   public readonly partial struct NestedStructTestValueObject;

   [ComplexValueObject]
   public sealed partial class NestedClassComplexValueObject;

   [ComplexValueObject]
   public readonly partial struct NestedStructComplexValueObject;

   public partial class ChildClass
   {
      [ValueObject<int>]
      public sealed partial class NestedClassTestValueObject2;

      [ValueObject<int>]
      public readonly partial struct NestedStructTestValueObject2;

      [ComplexValueObject]
      public sealed partial class NestedClassComplexValueObject2;

      [ComplexValueObject]
      public readonly partial struct NestedStructComplexValueObject2;
   }
}

public partial struct ParentStruct
{
   [ValueObject<int>]
   public sealed partial class NestedClassTestValueObject;

   [ValueObject<int>]
   public readonly partial struct NestedStructTestValueObject;

   [ComplexValueObject]
   public sealed partial class NestedClassComplexValueObject;

   [ComplexValueObject]
   public readonly partial struct NestedStructComplexValueObject;

   public partial class ChildClass
   {
      [ValueObject<int>]
      public sealed partial class NestedClassTestValueObject2;

      [ValueObject<int>]
      public readonly partial struct NestedStructTestValueObject2;

      [ComplexValueObject]
      public sealed partial class NestedClassComplexValueObject2;

      [ComplexValueObject]
      public readonly partial struct NestedStructComplexValueObject2;
   }
}

public partial record struct ParentRecordStruct
{
   [ValueObject<int>]
   public sealed partial class NestedClassTestValueObject;

   [ValueObject<int>]
   public readonly partial struct NestedStructTestValueObject;

   [ComplexValueObject]
   public sealed partial class NestedClassComplexValueObject;

   [ComplexValueObject]
   public readonly partial struct NestedStructComplexValueObject;

   public partial class ChildClass
   {
      [ValueObject<int>]
      public sealed partial class NestedClassTestValueObject2;

      [ValueObject<int>]
      public readonly partial struct NestedStructTestValueObject2;

      [ComplexValueObject]
      public sealed partial class NestedClassComplexValueObject2;

      [ComplexValueObject]
      public readonly partial struct NestedStructComplexValueObject2;
   }
}

public partial record ParentRecord
{
   [ValueObject<int>]
   public sealed partial class NestedClassTestValueObject;

   [ValueObject<int>]
   public readonly partial struct NestedStructTestValueObject;

   [ComplexValueObject]
   public sealed partial class NestedClassComplexValueObject;

   [ComplexValueObject]
   public readonly partial struct NestedStructComplexValueObject;

   public partial class ChildClass
   {
      [ValueObject<int>]
      public sealed partial class NestedClassTestValueObject2;

      [ValueObject<int>]
      public readonly partial struct NestedStructTestValueObject2;

      [ComplexValueObject]
      public sealed partial class NestedClassComplexValueObject2;

      [ComplexValueObject]
      public readonly partial struct NestedStructComplexValueObject2;
   }
}

// An interface as containing type is the fifth kind next to class, struct, record struct and record. It exists so
// that a type nested in an interface is rendered by the real generator pipeline on every build. Rendering the
// containing type as "partial class" instead of "partial interface" would not compile.
public partial interface IParentInterface
{
   [ValueObject<int>]
   public sealed partial class NestedClassTestValueObject;

   [ComplexValueObject]
   public sealed partial class NestedClassComplexValueObject;
}
