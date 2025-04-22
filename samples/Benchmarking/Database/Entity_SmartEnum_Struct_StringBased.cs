namespace Thinktecture.Database;

// ReSharper disable InconsistentNaming
public class Entity_SmartEnum_Struct_StringBased
{
   public int Id { get; set; }
   public required TestSmartEnum_Struct_StringBased Enum { get; set; }

   public Entity_SmartEnum_Struct_StringBased(int id, TestSmartEnum_Struct_StringBased @enum)
   {
      Id = id;
      Enum = @enum;
   }
}
