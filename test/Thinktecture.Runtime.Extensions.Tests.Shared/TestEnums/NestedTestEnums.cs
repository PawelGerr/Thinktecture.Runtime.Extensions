namespace Thinktecture.Runtime.Tests.TestEnums;

// ReSharper disable once InconsistentNaming
public partial class SmartEnums_Nested
{
   [SmartEnum]
   public sealed partial class NestedKeylessClassTestEnum
   {
      public static readonly NestedKeylessClassTestEnum Item = new();
   }

   [SmartEnum]
   public partial class NestedKeylessClassTestEnumWithDerivedType
   {
      public static readonly NestedKeylessClassTestEnumWithDerivedType Item = new();

      private sealed class DerivedType : NestedKeylessClassTestEnumWithDerivedType;
   }

   [SmartEnum<int>]
   public sealed partial class NestedIntBasedClassTestEnum
   {
      public static readonly NestedIntBasedClassTestEnum Item = new(1);
   }

   [SmartEnum<int>]
   public partial class NestedIntBasedClassTestEnumWithDerivedType
   {
      public static readonly NestedIntBasedClassTestEnumWithDerivedType Item = new(1);

      private sealed class DerivedType : NestedIntBasedClassTestEnumWithDerivedType
      {
         private DerivedType(int key)
            : base(key)
         {
         }
      }
   }

   [SmartEnum<int>]
   public sealed partial class NestedIntBasedSmartEnum
   {
      public static readonly NestedIntBasedSmartEnum Item = new(1);
   }

   public partial class ChildClass
   {
      [SmartEnum]
      public sealed partial class NestedKeylessSmartEnum2
      {
         public static readonly NestedKeylessSmartEnum2 Item = new();
      }

      [SmartEnum]
      public partial class NestedKeylessSmartEnumWithDerivedType2
      {
         public static readonly NestedKeylessSmartEnumWithDerivedType2 Item = new();

         private sealed class DerivedType : NestedKeylessSmartEnumWithDerivedType2;
      }

      [SmartEnum<int>]
      public sealed partial class NestedIntBasedClassTestEnum2
      {
         public static readonly NestedIntBasedClassTestEnum2 Item = new(1);
      }

      [SmartEnum<int>]
      public partial class NestedIntBasedClassTestEnumWithDerivedType2
      {
         public static readonly NestedIntBasedClassTestEnumWithDerivedType2 Item = new(1);

         private sealed class DerivedType : NestedIntBasedClassTestEnumWithDerivedType2
         {
            private DerivedType(int key)
               : base(key)
            {
            }
         }
      }

      [SmartEnum<int>]
      public sealed partial class NestedIntBasedSmartEnum2
      {
         public static readonly NestedIntBasedSmartEnum2 Item = new(1);
      }
   }
}

public partial struct ParentStruct
{
   [SmartEnum]
   public sealed partial class NestedKeylessClassTestEnum
   {
      public static readonly NestedKeylessClassTestEnum Item = new();
   }

   [SmartEnum]
   public partial class NestedKeylessClassTestEnumWithDerivedType
   {
      public static readonly NestedKeylessClassTestEnumWithDerivedType Item = new();

      private sealed class DerivedType : NestedKeylessClassTestEnumWithDerivedType;
   }

   [SmartEnum<int>]
   public sealed partial class NestedIntBasedClassTestEnum
   {
      public static readonly NestedIntBasedClassTestEnum Item = new(1);
   }

   [SmartEnum<int>]
   public partial class NestedIntBasedClassTestEnumWithDerivedType
   {
      public static readonly NestedIntBasedClassTestEnumWithDerivedType Item = new(1);

      private sealed class DerivedType : NestedIntBasedClassTestEnumWithDerivedType
      {
         private DerivedType(int key)
            : base(key)
         {
         }
      }
   }

   [SmartEnum<int>]
   public sealed partial class NestedIntBasedSmartEnum
   {
      public static readonly NestedIntBasedSmartEnum Item = new(1);
   }

   public partial class ChildClass
   {
      [SmartEnum]
      public sealed partial class NestedKeylessClassTestEnum2
      {
         public static readonly NestedKeylessClassTestEnum2 Item = new();
      }

      [SmartEnum]
      public partial class NestedKeylessClassTestEnumWithDerivedType2
      {
         public static readonly NestedKeylessClassTestEnumWithDerivedType2 Item = new();

         private sealed class DerivedType : NestedKeylessClassTestEnumWithDerivedType2;
      }

      [SmartEnum<int>]
      public sealed partial class NestedIntBasedClassTestEnum2
      {
         public static readonly NestedIntBasedClassTestEnum2 Item = new(1);
      }

      [SmartEnum<int>]
      public partial class NestedIntBasedClassTestEnumWithDerivedType2
      {
         public static readonly NestedIntBasedClassTestEnumWithDerivedType2 Item = new(1);

         private sealed class DerivedType : NestedIntBasedClassTestEnumWithDerivedType2
         {
            private DerivedType(int key)
               : base(key)
            {
            }
         }
      }

      [SmartEnum<int>]
      public sealed partial class NestedIntBasedSmartEnum2
      {
         public static readonly NestedIntBasedSmartEnum2 Item = new(1);
      }
   }
}

public partial record struct ParentRecordStruct
{
   [SmartEnum]
   public sealed partial class NestedKeylessClassTestEnum
   {
      public static readonly NestedKeylessClassTestEnum Item = new();
   }

   [SmartEnum]
   public partial class NestedKeylessClassTestEnumWithDerivedType
   {
      public static readonly NestedKeylessClassTestEnumWithDerivedType Item = new();

      private sealed class DerivedType : NestedKeylessClassTestEnumWithDerivedType;
   }

   [SmartEnum<int>]
   public sealed partial class NestedIntBasedClassTestEnum
   {
      public static readonly NestedIntBasedClassTestEnum Item = new(1);
   }

   [SmartEnum<int>]
   public partial class NestedIntBasedClassTestEnumWithDerivedType
   {
      public static readonly NestedIntBasedClassTestEnumWithDerivedType Item = new(1);

      private sealed class DerivedType : NestedIntBasedClassTestEnumWithDerivedType
      {
         private DerivedType(int key)
            : base(key)
         {
         }
      }
   }

   [SmartEnum<int>]
   public sealed partial class NestedIntBasedSmartEnum
   {
      public static readonly NestedIntBasedSmartEnum Item = new(1);
   }

   public partial class ChildClass
   {
      [SmartEnum]
      public sealed partial class NestedKeylessClassTestEnum2
      {
         public static readonly NestedKeylessClassTestEnum2 Item = new();
      }

      [SmartEnum]
      public partial class NestedKeylessClassTestEnumWithDerivedType2
      {
         public static readonly NestedKeylessClassTestEnumWithDerivedType2 Item = new();

         private sealed class DerivedType : NestedKeylessClassTestEnumWithDerivedType2;
      }

      [SmartEnum<int>]
      public sealed partial class NestedIntBasedClassTestEnum2
      {
         public static readonly NestedIntBasedClassTestEnum2 Item = new(1);
      }

      [SmartEnum<int>]
      public partial class NestedIntBasedClassTestEnumWithDerivedType2
      {
         public static readonly NestedIntBasedClassTestEnumWithDerivedType2 Item = new(1);

         private sealed class DerivedType : NestedIntBasedClassTestEnumWithDerivedType2
         {
            private DerivedType(int key)
               : base(key)
            {
            }
         }
      }

      [SmartEnum<int>]
      public sealed partial class NestedIntBasedSmartEnum2
      {
         public static readonly NestedIntBasedSmartEnum2 Item = new(1);
      }
   }
}

public partial record ParentRecord
{
   [SmartEnum]
   public sealed partial class NestedKeylessClassTestEnum
   {
      public static readonly NestedKeylessClassTestEnum Item = new();
   }

   [SmartEnum]
   public partial class NestedKeylessClassTestEnumWithDerivedType
   {
      public static readonly NestedKeylessClassTestEnumWithDerivedType Item = new();

      private sealed class DerivedType : NestedKeylessClassTestEnumWithDerivedType;
   }

   [SmartEnum<int>]
   public sealed partial class NestedIntBasedClassTestEnum
   {
      public static readonly NestedIntBasedClassTestEnum Item = new(1);
   }

   [SmartEnum<int>]
   public partial class NestedIntBasedClassTestEnumWithDerivedType
   {
      public static readonly NestedIntBasedClassTestEnumWithDerivedType Item = new(1);

      private sealed class DerivedType : NestedIntBasedClassTestEnumWithDerivedType
      {
         private DerivedType(int key)
            : base(key)
         {
         }
      }
   }

   [SmartEnum<int>]
   public sealed partial class NestedIntBasedSmartEnum
   {
      public static readonly NestedIntBasedSmartEnum Item = new(1);
   }

   public partial class ChildClass
   {
      [SmartEnum]
      public sealed partial class NestedKeylessClassTestEnum2
      {
         public static readonly NestedKeylessClassTestEnum2 Item = new();
      }

      [SmartEnum]
      public partial class NestedKeylessClassTestEnumWithDerivedType2
      {
         public static readonly NestedKeylessClassTestEnumWithDerivedType2 Item = new();

         private sealed class DerivedType : NestedKeylessClassTestEnumWithDerivedType2;
      }

      [SmartEnum<int>]
      public sealed partial class NestedIntBasedClassTestEnum2
      {
         public static readonly NestedIntBasedClassTestEnum2 Item = new(1);
      }

      [SmartEnum<int>]
      public partial class NestedIntBasedClassTestEnumWithDerivedType2
      {
         public static readonly NestedIntBasedClassTestEnumWithDerivedType2 Item = new(1);

         private sealed class DerivedType : NestedIntBasedClassTestEnumWithDerivedType2
         {
            private DerivedType(int key)
               : base(key)
            {
            }
         }
      }

      [SmartEnum<int>]
      public sealed partial class NestedIntBasedSmartEnum2
      {
         public static readonly NestedIntBasedSmartEnum2 Item = new(1);
      }
   }
}
