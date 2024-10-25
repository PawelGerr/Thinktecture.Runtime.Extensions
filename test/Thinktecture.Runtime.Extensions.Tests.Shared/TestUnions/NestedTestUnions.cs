namespace Thinktecture.Runtime.Tests.TestUnions;

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
