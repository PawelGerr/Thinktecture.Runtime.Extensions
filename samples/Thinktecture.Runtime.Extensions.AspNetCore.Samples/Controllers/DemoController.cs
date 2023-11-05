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

   [HttpGet("productType")]
   public IActionResult RoundTripWithQueryString(ProductType productType)
   {
      return RoundTrip<ProductType, string>(productType);
   }

   [HttpGet("boundary/{boundary}")]
   public IActionResult RoundTrip(Boundary boundary)
   {
      if (!ModelState.IsValid)
         return BadRequest(ModelState);

      _logger.LogInformation("Round trip test with {Type}: {Boundary}", boundary.GetType().Name, boundary);

      return Json(boundary);
   }

   [HttpPost("productType")]
   public IActionResult RoundTripPost([FromBody] ProductType productType)
   {
      return RoundTrip<ProductType, string>(productType);
   }

   [HttpPost("productTypeWrapper")]
   public IActionResult RoundTripPost([FromBody] ProductTypeWrapper productType)
   {
      return RoundTrip<ProductType, string>(productType.ProductType);
   }

   [HttpGet("productTypeWithJsonConverter/{productType}")]
   public IActionResult RoundTrip(ProductTypeWithJsonConverter productType)
   {
      return RoundTrip<ProductTypeWithJsonConverter, string>(productType);
   }

   [HttpGet("productName/{name}")]
   public IActionResult RoundTrip(ProductName? name)
   {
      if (!ModelState.IsValid)
         return BadRequest(ModelState);

      _logger.LogInformation("Round trip test with {Type}: {Name}", name?.GetType().Name, name);

      return Json(name);
   }

   [HttpPost("productName")]
   public IActionResult RoundTripPost([FromBody] ProductName? name)
   {
      if (!ModelState.IsValid)
         return BadRequest(ModelState);

      _logger.LogInformation("Round trip test with {Type}: {Name}", name?.GetType().Name, name);

      return Json(name);
   }

   [HttpGet("otherProductName/{name}")]
   public IActionResult RoundTrip(OtherProductName? name)
   {
      if (!ModelState.IsValid)
         return BadRequest(ModelState);

      _logger.LogInformation("Round trip test with {Type}: {Name}", name?.GetType().Name, name);

      return Json(name);
   }

   [HttpPost("otherProductName")]
   public IActionResult RoundTripPost([FromBody] OtherProductName? name)
   {
      if (!ModelState.IsValid)
         return BadRequest(ModelState);

      _logger.LogInformation("Round trip test with {Type}: {Name}", name?.GetType().Name, name);

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

   [HttpGet("enddate/{endDate}")]
   public IActionResult RoundTripGet(EndDate endDate)
   {
      if (!ModelState.IsValid)
         return BadRequest(ModelState);

      _logger.LogInformation("Round trip test with {Type}: {EndDate}", endDate.GetType().Name, endDate);

      return Json(endDate);
   }

   [HttpPost("enddate")]
   public IActionResult RoundTripPost([FromBody] EndDate endDate)
   {
      if (!ModelState.IsValid)
         return BadRequest(ModelState);

      _logger.LogInformation("Round trip test with {Type}: {EndDate}", endDate.GetType().Name, endDate);

      return Json(endDate);
   }

   private IActionResult RoundTripValidatableEnum<T, TKey>(T value)
      where T : IValidatableEnum<TKey>
      where TKey : notnull
   {
      if (!ModelState.IsValid)
         return BadRequest(ModelState);

      _logger.LogInformation("Round trip test with {Type}: {Name}", value.GetType().Name, value);

      return Json(new { Value = value, value.IsValid });
   }

   private IActionResult RoundTrip<T, TKey>(T value)
      where T : IEnum<TKey>
      where TKey : notnull
   {
      if (!ModelState.IsValid)
         return BadRequest(ModelState);

      _logger.LogInformation("Round trip test with {Type}: {Name}", value.GetType().Name, value);

      return Json(value);
   }
}
