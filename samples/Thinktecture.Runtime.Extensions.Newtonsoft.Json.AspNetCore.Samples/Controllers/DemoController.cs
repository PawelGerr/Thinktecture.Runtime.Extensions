using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Thinktecture.Runtime.Extensions.Samples.EnumLikeClass;

namespace Thinktecture.Controllers
{
   [Route("api")]
   public class DemoController : Controller
   {
      private readonly ILogger<DemoController> _logger;

      public DemoController(ILogger<DemoController> logger)
      {
         _logger = logger ?? throw new ArgumentNullException(nameof(logger));
      }

      [HttpGet("category/{category}")]
      public IActionResult RoundTrip(ProductCategory category)
      {
         return RoundTrip<ProductCategory>(category);
      }

      [HttpGet("categoryWithConverter/{category}")]
      public IActionResult RoundTrip(ProductCategoryWithJsonConverter category)
      {
         return RoundTrip<ProductCategoryWithJsonConverter>(category);
      }

      [HttpGet("group/{group}")]
      public IActionResult RoundTrip(ProductGroup group)
      {
         return RoundTrip<ProductGroup>(group);
      }

      [HttpGet("groupWithConverter/{group}")]
      public IActionResult RoundTrip(ProductGroupWithJsonConverter group)
      {
         return RoundTrip<ProductGroupWithJsonConverter>(group);
      }

      private IActionResult RoundTrip<T>(T value)
         where T : IEnum
      {
         _logger.LogInformation($"Round trip test with {value.GetType().Name}", value);

         return Json(new { Value = value, value.IsValid });
      }
   }
}
