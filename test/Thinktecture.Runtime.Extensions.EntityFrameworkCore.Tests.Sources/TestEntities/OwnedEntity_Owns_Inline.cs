using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.TestEntities;

// ReSharper disable InconsistentNaming
public class OwnedEntity_Owns_Inline
{
   public int IntProp { get; set; }

   public SmartEnum_StringBased TestEnum { get; set; }

   public OwnedEntity InlineEntity { get; set; }
}
