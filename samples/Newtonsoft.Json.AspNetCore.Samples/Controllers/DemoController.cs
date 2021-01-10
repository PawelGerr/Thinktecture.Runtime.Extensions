using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Thinktecture.EnumLikeClasses;

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
         return RoundTripValidatableEnum<ProductCategory, string>(category);
      }

      [HttpGet("categoryWithConverter/{category}")]
      public IActionResult RoundTrip(ProductCategoryWithJsonConverter category)
      {
         return RoundTripValidatableEnum<ProductCategoryWithJsonConverter, string>(category);
      }

      [HttpGet("group/{group}")]
      public IActionResult RoundTrip(ProductGroup group)
      {
         return RoundTripValidatableEnum<ProductGroup, int>(group);
      }

      [HttpGet("groupWithConverter/{group}")]
      public IActionResult RoundTrip(ProductGroupWithJsonConverter group)
      {
         return RoundTripValidatableEnum<ProductGroupWithJsonConverter, int>(group);
      }

      [HttpGet("productType/{productType}")]
      public IActionResult RoundTrip(ProductType productType)
      {
         return RoundTrip<ProductType, string>(productType);
      }

      private IActionResult RoundTripValidatableEnum<T, TKey>(T value)
         where T : IValidatableEnum<TKey>
         where TKey : notnull
      {
         if (!ModelState.IsValid)
            return BadRequest(ModelState);

         _logger.LogInformation($"Round trip test with {value.GetType().Name}", value);

         return Json(new { Value = value, value.IsValid });
      }

      private IActionResult RoundTrip<T, TKey>(T value)
         where T : IEnum<TKey>
         where TKey : notnull
      {
         if (!ModelState.IsValid)
            return BadRequest(ModelState);

         _logger.LogInformation($"Round trip test with {value.GetType().Name}", value);

         return Json(value);
      }
   }
}
