using System;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Thinktecture.Runtime.Tests.Swashbuckle.Helpers;

public class TestControllerFeatureProvider(Type controllerType) : ControllerFeatureProvider
{
   protected override bool IsController(TypeInfo typeInfo)
   {
      return typeInfo == controllerType;
   }
}
