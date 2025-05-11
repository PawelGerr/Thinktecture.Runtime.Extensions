using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Thinktecture.SmartEnums;
using Thinktecture.ValueObjects;

namespace Thinktecture.Controllers;

[Route("api"), ApiController]
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
      return RoundTripInternal(category);
   }

   [HttpGet("categoryWithConverter/{category}")]
   public IActionResult RoundTrip(ProductCategoryWithJsonConverter category)
   {
      return RoundTripInternal(category);
   }

   [HttpGet("group/{group}")]
   public IActionResult RoundTrip(ProductGroup group)
   {
      return RoundTripInternal(group);
   }

   [HttpGet("groupWithConverter/{group}")]
   public IActionResult RoundTrip(ProductGroupWithJsonConverter group)
   {
      return RoundTripInternal(group);
   }

   [HttpGet("productType/{productType}")]
   public IActionResult RoundTrip(ProductType productType)
   {
      return RoundTripInternal(productType);
   }

   [HttpPost("productType")]
   public IActionResult RoundTripPost([FromBody] ProductType productType)
   {
      return RoundTripInternal(productType);
   }

   public record ProductTypeWrapper(ProductType ProductType);

   [HttpPost("productTypeWrapper")]
   public IActionResult RoundTripPost([FromBody] ProductTypeWrapper productType)
   {
      return RoundTripInternal(productType.ProductType);
   }

   [HttpGet("productTypeWithJsonConverter/{productType}")]
   public IActionResult RoundTrip(ProductTypeWithJsonConverter productType)
   {
      return RoundTripInternal(productType);
   }

   [HttpGet("productName/{name}")]
   public IActionResult RoundTrip(ProductName name)
   {
      if (!ModelState.IsValid)
         return BadRequest(ModelState);

      _logger.LogInformation("Round trip test with {Type}: {Name}", name.GetType().Name, name);

      return Json(name);
   }

   [HttpPost("boundary")]
   public IActionResult RoundTrip([FromBody] BoundaryWithJsonConverter boundary)
   {
      if (!ModelState.IsValid)
         return BadRequest(ModelState);

      _logger.LogInformation("Round trip test with {Type}: {Boundary}", boundary.GetType().Name, boundary);

      return Json(boundary);
   }

   private IActionResult RoundTripInternal<T>(T value)
      where T : notnull
   {
      if (!ModelState.IsValid)
         return BadRequest(ModelState);

      _logger.LogInformation("Round trip test with {Type}: {Value}", value.GetType().Name, value);

      return Json(value);
   }
}
