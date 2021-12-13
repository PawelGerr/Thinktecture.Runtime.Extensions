using System.Collections.Generic;
using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.TestEntities;
#pragma warning disable 8618
public class OwnedEntity_Owns_SeparateMany
{
   public TestEnum TestEnum { get; set; }

   public List<OwnedEntity> SeparateEntities { get; set; }
}