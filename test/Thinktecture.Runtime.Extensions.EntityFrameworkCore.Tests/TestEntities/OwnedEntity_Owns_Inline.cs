// ReSharper disable InconsistentNaming
using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.TestEntities
{
#pragma warning disable 8618
   public class OwnedEntity_Owns_Inline
   {
      public int IntProp { get; set; }

      public TestEnum TestEnum { get; set; }

      public OwnedEntity InlineEntity { get; set; }
   }
}
