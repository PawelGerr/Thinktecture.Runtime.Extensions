namespace Thinktecture.Database;

// ReSharper disable InconsistentNaming
public class Entity_SmartEnum_Class_StringBased
{
   public int Id { get; set; }
   public TestSmartEnum_Class_StringBased Enum { get; set; }

   public Entity_SmartEnum_Class_StringBased(int id, TestSmartEnum_Class_StringBased @enum)
   {
      Id = id;
      Enum = @enum;
   }
}
