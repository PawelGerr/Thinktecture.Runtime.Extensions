using System;
using MessagePack;
using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.Formatters.EnumMessagePackFormatterTests.TestClasses;

[MessagePackObject]
public class ClassWithIntBasedEnum : IEquatable<ClassWithIntBasedEnum>
{
   [Key(0)]
   public IntegerEnum Enum { get; set; }

   public ClassWithIntBasedEnum()
   {
   }

   public ClassWithIntBasedEnum(IntegerEnum value)
   {
      Enum = value;
   }

   public override bool Equals(object obj)
   {
      return obj is ClassWithIntBasedEnum classWithEnum
             && Equals(classWithEnum);
   }

   public bool Equals(ClassWithIntBasedEnum other)
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
