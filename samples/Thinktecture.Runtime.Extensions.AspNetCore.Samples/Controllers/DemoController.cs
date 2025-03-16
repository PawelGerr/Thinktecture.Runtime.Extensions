using System;
using System.Linq;
using System.Threading.Tasks;
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
      return RoundTripValidatableEnum(category);
   }

   [HttpGet("group/{group}")]
   public IActionResult RoundTrip(ProductGroup group)
   {
      return RoundTripValidatableEnum(group);
   }

   [HttpGet("productType/{productType}")]
   public IActionResult RoundTrip(ProductType productType)
   {
      return RoundTripInternal(productType);
   }

   [HttpGet("productType")]
   public IActionResult RoundTripWithQueryString(ProductType productType)
   {
      return RoundTripInternal(productType);
   }

   [HttpGet("boundaryWithFactories/{boundary}")]
   public IActionResult RoundTrip(BoundaryWithFactories boundary)
   {
      if (!ModelState.IsValid)
         return BadRequest(ModelState);

      _logger.LogInformation("Round trip test with {Type}: {Boundary}", boundary.GetType().Name, boundary);

      return Json(boundary);
   }

   [HttpPost("productType")]
   public IActionResult RoundTripPost([FromBody] ProductType productType)
   {
      return RoundTripInternal(productType);
   }

   [HttpPost("productTypeWrapper")]
   public IActionResult RoundTripPost([FromBody] ProductTypeWrapper productType)
   {
      return RoundTripInternal(productType.ProductType);
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

   [HttpGet("enddate/{endDate}")]
   public IActionResult RoundTripGet(OpenEndDate endDate)
   {
      if (!ModelState.IsValid)
         return BadRequest(ModelState);

      _logger.LogInformation("Round trip test with {Type}: {EndDate}", endDate.GetType().Name, endDate);

      return Json(endDate);
   }

   [HttpPost("enddate")]
   public IActionResult RoundTripPost([FromBody] OpenEndDate endDate)
   {
      if (!ModelState.IsValid)
         return BadRequest(ModelState);

      _logger.LogInformation("Round trip test with {Type}: {EndDate}", endDate.GetType().Name, endDate);

      return Json(endDate);
   }

   private IActionResult RoundTripValidatableEnum<T>(T value)
      where T : IValidatableEnum
   {
      if (!ModelState.IsValid)
         return BadRequest(ModelState);

      _logger.LogInformation("Round trip test with {Type}: {Value}", value.GetType().Name, value);

      return Json(new { Value = value, value.IsValid });
   }

   private IActionResult RoundTripInternal<T>(T value)
      where T : notnull
   {
      if (!ModelState.IsValid)
         return BadRequest(ModelState);

      _logger.LogInformation("Round trip test with {Type}: {Value}", value.GetType().Name, value);

      return Json(value);
   }

   [HttpGet("notification/channels")]
   public IActionResult GetAvailableChannels()
   {
      // Return all available notification channels
      var channels = NotificationChannelTypeDto.Items.Select(c => c.Name);
      return Ok(channels);
   }

   [HttpPost("notification/channels/{type}")]
   public async Task<IActionResult> SendNotificationAsync(
      NotificationChannelTypeDto type,
      [FromBody] string message,
      [FromServices] IServiceProvider serviceProvider)
   {
      var notificationSender = type.GetNotificationSender(serviceProvider);
      await notificationSender.SendAsync(message);

      return Ok();
   }
}
