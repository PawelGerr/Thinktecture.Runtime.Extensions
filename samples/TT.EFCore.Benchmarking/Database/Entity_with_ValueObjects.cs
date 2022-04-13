namespace Thinktecture.Database;

public class Entity_with_ValueObjects
{
   public int Id { get; set; }
   public Name Name { get; set; }
   public Description Description { get; set; }

   private Entity_with_ValueObjects(int id, Name name, Description description)
   {
      Id = id;
      Name = name;
      Description = description;
   }

   public Entity_with_ValueObjects(int index)
   {
      Id = index;
      Name = Name.Create($"Name {index}");
      Description = Description.Create($"Description {index}");
   }
}
