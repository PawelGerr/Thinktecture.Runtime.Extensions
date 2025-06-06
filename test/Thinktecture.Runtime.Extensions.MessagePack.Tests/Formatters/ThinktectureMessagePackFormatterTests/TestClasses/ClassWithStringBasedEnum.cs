using System;
using MessagePack;
using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.Formatters.ThinktectureMessagePackFormatterTests.TestClasses;

[MessagePackObject]
public class ClassWithStringBasedEnum : IEquatable<ClassWithStringBasedEnum>
{
   [Key(0)]
   public SmartEnum_StringBased Enum { get; init; }

   public ClassWithStringBasedEnum()
   {
   }

   public ClassWithStringBasedEnum(SmartEnum_StringBased value)
   {
      Enum = value;
   }

   public override bool Equals(object obj)
   {
      return obj is ClassWithStringBasedEnum classWithEnum
             && Equals(classWithEnum);
   }

   public bool Equals(ClassWithStringBasedEnum other)
   {
      if (ReferenceEquals(null, other))
         return false;
      if (ReferenceEquals(this, other))
         return true;
      return Equals(Enum, other.Enum);
   }

   public override int GetHashCode()
   {
      return Enum?.GetHashCode() ?? 0;
   }
}
