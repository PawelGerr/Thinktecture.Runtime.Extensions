using System;

namespace Thinktecture.Runtime.Tests.TestEnums;

[EnumGeneration(IsExtensible = true)]
public partial class ExtensibleTestValidatableEnum : IValidatableEnum<string>
{
   public static readonly ExtensibleTestValidatableEnum Item1 = new("Item1", Empty.Action);

   [EnumGenerationMember(MapsToMember = nameof(Foo))]
   private readonly Action _foo;

   public void Foo()
   {
      _foo();
   }
}