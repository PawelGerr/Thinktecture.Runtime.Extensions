namespace Thinktecture.Database;

// ReSharper disable InconsistentNaming
public class Entity_SmartEnum_Struct_IntBased
{
   public int Id { get; set; }
   public TestSmartEnum_Struct_IntBased Enum { get; set; }

   // ReSharper disable once UnassignedGetOnlyAutoProperty
   public int Foo { get; }

   public Entity_SmartEnum_Struct_IntBased(int id, TestSmartEnum_Struct_IntBased @enum)
   {
      Id = id;
      Enum = @enum;
   }
}
