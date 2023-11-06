namespace Thinktecture.Database;

// ReSharper disable InconsistentNaming
public class Entity_with_StructValueObjects
{
   public int Id { get; set; }
   public NameStruct Name { get; set; }
   public DescriptionStruct Description { get; set; }

   // ReSharper disable once UnusedMember.Local
   private Entity_with_StructValueObjects(int id, NameStruct name, DescriptionStruct description)
   {
      Id = id;
      Name = name;
      Description = description;
   }

   public Entity_with_StructValueObjects(int index)
   {
      Id = index;
      Name = NameStruct.Create($"Name {index}");
      Description = DescriptionStruct.Create($"Description {index}");
   }
}
