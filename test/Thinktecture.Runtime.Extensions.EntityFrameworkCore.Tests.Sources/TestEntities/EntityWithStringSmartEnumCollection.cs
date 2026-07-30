using System;
using System.Collections.Generic;
using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.TestEntities;

public class EntityWithStringSmartEnumCollection
{
   public Guid Id { get; set; }
   public List<SmartEnum_StringBased> Items { get; set; } = [];
}
