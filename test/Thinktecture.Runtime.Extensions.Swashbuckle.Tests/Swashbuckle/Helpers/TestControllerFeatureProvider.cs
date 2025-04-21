using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Thinktecture.Runtime.Tests.Swashbuckle.Helpers;

public class TestControllerFeatureProvider : ControllerFeatureProvider
{
   protected override bool IsController(TypeInfo typeInfo)
   {
      return typeof(ControllerBase).IsAssignableFrom(typeInfo);
   }
}
