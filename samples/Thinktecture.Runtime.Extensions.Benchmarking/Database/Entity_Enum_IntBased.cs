namespace Thinktecture.Database;

public class Entity_Enum_IntBased
{
   public int Id { get; set; }
   public RealEnum Enum { get; set; }

   public Entity_Enum_IntBased(int id, RealEnum @enum)
   {
      Id = id;
      Enum = @enum;
   }
}
