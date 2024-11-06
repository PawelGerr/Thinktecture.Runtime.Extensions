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
