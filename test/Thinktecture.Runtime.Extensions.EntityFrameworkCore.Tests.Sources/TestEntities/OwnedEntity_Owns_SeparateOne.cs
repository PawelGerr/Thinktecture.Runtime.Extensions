using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.TestEntities;

// ReSharper disable InconsistentNaming
public class OwnedEntity_Owns_SeparateOne
{
   public TestEnum TestEnum { get; set; }

   public OwnedEntity SeparateEntity { get; set; }
}
