namespace Thinktecture.Runtime.Tests.TestEnums;

[EnumGeneration(IsExtensible = true, KeyPropertyName = "Id", KeyComparer = nameof(EqualityComparer))]
public partial class ExtensibleTestEnum : IEnum<string>
{
   protected static readonly IEqualityComparer<string> EqualityComparer = StringComparer.OrdinalIgnoreCase;

   public static readonly ExtensibleTestEnum DerivedItem = new ExtensibleTestEnumImpl("DerivedItem", Empty.Action);
   public static readonly ExtensibleTestEnum Item1 = new("Item1", Empty.Action);

   [EnumGenerationMember(MapsToMember = nameof(Foo))]
   private readonly Action _foo;

   public void Foo()
   {
      _foo();
   }

   private class ExtensibleTestEnumImpl : ExtensibleTestEnum
   {
      public ExtensibleTestEnumImpl(string key, Action foo)
         : base(key, foo)
      {
      }
   }
}
