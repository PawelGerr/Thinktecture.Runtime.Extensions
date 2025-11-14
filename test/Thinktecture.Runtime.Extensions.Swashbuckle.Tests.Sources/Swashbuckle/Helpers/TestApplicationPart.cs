using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace Thinktecture.Runtime.Tests.Swashbuckle.Helpers;

public class TestApplicationPart : ApplicationPart, IApplicationPartTypeProvider
{
   private readonly Type _controllerType;

   public override string Name => "TestPart";
   public IEnumerable<TypeInfo> Types => [_controllerType.GetTypeInfo()];

   public TestApplicationPart(Type controllerType)
   {
      if (!typeof(ControllerBase).IsAssignableFrom(controllerType))
         throw new ArgumentException($"Type '{controllerType}' is not a controller type.", nameof(controllerType));

      _controllerType = controllerType;
   }
}
