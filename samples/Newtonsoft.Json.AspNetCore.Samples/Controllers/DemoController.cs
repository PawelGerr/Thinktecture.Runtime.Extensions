using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Thinktecture.EnumLikeClass;

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
         return RoundTrip<ProductCategory, string>(category);
      }

      [HttpGet("categoryWithConverter/{category}")]
      public IActionResult RoundTrip(ProductCategoryWithJsonConverter category)
      {
         return RoundTrip<ProductCategoryWithJsonConverter, string>(category);
      }

      [HttpGet("group/{group}")]
      public IActionResult RoundTrip(ProductGroup group)
      {
         return RoundTrip<ProductGroup, int>(group);
      }

      [HttpGet("groupWithConverter/{group}")]
      public IActionResult RoundTrip(ProductGroupWithJsonConverter group)
      {
         return RoundTrip<ProductGroupWithJsonConverter, int>(group);
      }

      private IActionResult RoundTrip<T, TKey>(T value)
         where T : IValidatableEnum<TKey>
         where TKey : notnull
      {
         _logger.LogInformation($"Round trip test with {value.GetType().Name}", value);

         return Json(new { Value = value, value.IsValid });
      }
   }
}
