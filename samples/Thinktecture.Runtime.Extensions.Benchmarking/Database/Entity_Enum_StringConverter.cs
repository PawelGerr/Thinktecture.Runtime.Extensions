namespace Thinktecture.Database;

// ReSharper disable InconsistentNaming
public class Entity_Enum_StringConverter
{
   public int Id { get; set; }
   public RealEnum Enum { get; set; }

   public Entity_Enum_StringConverter(int id, RealEnum @enum)
   {
      Id = id;
      Enum = @enum;
   }
}
