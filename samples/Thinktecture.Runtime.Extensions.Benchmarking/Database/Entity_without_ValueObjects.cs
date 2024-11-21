namespace Thinktecture.Database;

// ReSharper disable InconsistentNaming
public class Entity_without_ValueObjects
{
   public int Id { get; set; }
   public string Name { get; set; }
   public string Description { get; set; }

   // ReSharper disable once UnusedMember.Local
   private Entity_without_ValueObjects(int id, string name, string description)
   {
      Id = id;
      Name = name;
      Description = description;
   }

   public Entity_without_ValueObjects(int index)
   {
      Id = index;
      Name = $"Name {index}";
      Description = $"Description {index}";
   }
}
