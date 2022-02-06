using System.Collections.Generic;
using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.TestEntities;
public class OwnedEntity_Owns_SeparateMany
{
   public TestEnum TestEnum { get; set; }

   public List<OwnedEntity> SeparateEntities { get; set; }
}
