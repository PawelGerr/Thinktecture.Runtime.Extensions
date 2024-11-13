namespace Thinktecture.Runtime.Tests.TestAdHocUnions;

public partial class ParentClass
{
   // ReSharper disable once InconsistentNaming
   [Union<string, int>]
   public partial class NestedTestUnion_class_string_int;

   public partial class ChildClass
   {
      // ReSharper disable once InconsistentNaming
      [Union<string, int>]
      public partial class NestedTestUnion_class_string_int2;
   }
}

public partial struct ParentStruct
{
   // ReSharper disable once InconsistentNaming
   [Union<string, int>]
   public partial class NestedTestUnion_class_string_int;

   public partial class ChildClass
   {
      // ReSharper disable once InconsistentNaming
      [Union<string, int>]
      public partial class NestedTestUnion_class_string_int2;
   }
}

public partial record struct ParentRecordStruct
{
   // ReSharper disable once InconsistentNaming
   [Union<string, int>]
   public partial class NestedTestUnion_class_string_int;

   public partial class ChildClass
   {
      // ReSharper disable once InconsistentNaming
      [Union<string, int>]
      public partial class NestedTestUnion_class_string_int2;
   }
}

public partial record ParentRecord
{
   // ReSharper disable once InconsistentNaming
   [Union<string, int>]
   public partial class NestedTestUnion_class_string_int;

   public partial class ChildClass
   {
      // ReSharper disable once InconsistentNaming
      [Union<string, int>]
      public partial class NestedTestUnion_class_string_int2;
   }
}
