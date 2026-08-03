using MessagePack;
using MessagePack.Formatters;

namespace Thinktecture.Runtime.Tests.Formatters.ThinktectureMessagePackFormatterTests.TestClasses;

[SmartEnum<int>]
public partial class IntBasedEnumWithFormatterOnDerivedType
{
   // The formatter adds this offset to the key so a test can prove, from the payload alone, that the formatter
   // declared on the derived type was used instead of the resolver's key-based wrapper.
   public const int DerivedItemKeyOffset = 1000;

   public static readonly IntBasedEnumWithFormatterOnDerivedType Item = new(1);
   public static readonly IntBasedEnumWithFormatterOnDerivedType ItemOfDerivedType = new DerivedItem(2);

   [MessagePackFormatter(typeof(DerivedItemFormatter))]
   private sealed class DerivedItem : IntBasedEnumWithFormatterOnDerivedType
   {
      public DerivedItem(int key)
         : base(key)
      {
      }
   }

   private sealed class DerivedItemFormatter : IMessagePackFormatter<DerivedItem>
   {
      public void Serialize(ref MessagePackWriter writer, DerivedItem value, MessagePackSerializerOptions options)
      {
         if (value is null)
         {
            writer.WriteNil();
            return;
         }

         writer.WriteInt32(value.Key + DerivedItemKeyOffset);
      }

      public DerivedItem Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
      {
         if (reader.TryReadNil())
            return null;

         return (DerivedItem)Get(reader.ReadInt32() - DerivedItemKeyOffset);
      }
   }
}
