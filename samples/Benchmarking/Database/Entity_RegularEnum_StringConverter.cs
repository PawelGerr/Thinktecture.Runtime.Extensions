namespace Thinktecture.Database;

// ReSharper disable InconsistentNaming
public class Entity_RegularEnum_StringConverter
{
   public int Id { get; set; }
   public RealEnum Enum { get; set; }

   public Entity_RegularEnum_StringConverter(int id, RealEnum @enum)
   {
      Id = id;
      Enum = @enum;
   }
}
