namespace Thinktecture.Database;

// ReSharper disable InconsistentNaming
public class Entity_SmartEnum_IntBased
{
   public int Id { get; set; }
   public TestSmartEnum_Class_IntBased Enum { get; set; }

   public Entity_SmartEnum_IntBased(int id, TestSmartEnum_Class_IntBased @enum)
   {
      Id = id;
      Enum = @enum;
   }
}
