using System;

namespace Thinktecture.Runtime.Tests.TestValueObjects;

[ComplexValueObject(DefaultStringComparison = StringComparison.OrdinalIgnoreCase)]
public partial class ComplexValueObjectWithIgnoredMembers
{
   public string Property1 { get; }

   [IgnoreMember]
   public string? IgnoredProperty1 { get; }

   public int Property2 { get; }

   [IgnoreMember]
   public int IgnoredProperty2 { get; }
}
