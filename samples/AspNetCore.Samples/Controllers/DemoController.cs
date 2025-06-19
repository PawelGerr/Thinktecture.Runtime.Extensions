using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Thinktecture.SmartEnums;
using Thinktecture.Unions;
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
   public ActionResult<ProductCategory> RoundTrip(ProductCategory category)
   {
      return RoundTripInternal(category);
   }

   [HttpGet("group/{group}")]
   public ActionResult<ProductGroup> RoundTrip(ProductGroup group)
   {
      return RoundTripInternal(group);
   }

   [HttpGet("productType/{productType}")]
   public ActionResult<ProductType> RoundTrip(ProductType productType)
   {
      return RoundTripInternal(productType);
   }

   [HttpGet("productType")]
   public ActionResult<ProductType> RoundTripWithQueryString(ProductType productType)
   {
      return RoundTripInternal(productType);
   }

   [HttpGet("boundaryWithFactories/{boundary}")]
   public ActionResult<BoundaryWithFactories> RoundTrip(BoundaryWithFactories boundary)
   {
      if (!ModelState.IsValid)
         return BadRequest(ModelState);

      _logger.LogInformation("Round trip test with {Type}: {Boundary}", boundary.GetType().Name, boundary);

      return Json(boundary);
   }

   [HttpPost("productType")]
   public ActionResult<ProductType> RoundTripPost([FromBody] ProductType productType)
   {
      return RoundTripInternal(productType);
   }

   [HttpPost("productTypeWrapper")]
   public ActionResult<ProductType> RoundTripPost([FromBody] ProductTypeWrapper productType)
   {
      return RoundTripInternal(productType.ProductType);
   }

   [HttpGet("productName/{name}")]
   public ActionResult<ProductName?> RoundTrip(ProductName? name)
   {
      if (!ModelState.IsValid)
         return BadRequest(ModelState);

      _logger.LogInformation("Round trip test with {Type}: {Name}", name?.GetType().Name, name);

      return Json(name);
   }

   [HttpPost("productName")]
   public ActionResult<ProductName?> RoundTripPost([FromBody] ProductName? name)
   {
      if (!ModelState.IsValid)
         return BadRequest(ModelState);

      _logger.LogInformation("Round trip test with {Type}: {Name}", name?.GetType().Name, name);

      return Json(name);
   }

   [HttpGet("otherProductName/{name}")]
   public ActionResult<OtherProductName?> RoundTrip(OtherProductName? name)
   {
      if (!ModelState.IsValid)
         return BadRequest(ModelState);

      _logger.LogInformation("Round trip test with {Type}: {Name}", name?.GetType().Name, name);

      return Json(name);
   }

   [HttpPost("otherProductName")]
   public ActionResult<OtherProductName?> RoundTripPost([FromBody] OtherProductName? name)
   {
      if (!ModelState.IsValid)
         return BadRequest(ModelState);

      _logger.LogInformation("Round trip test with {Type}: {Name}", name?.GetType().Name, name);

      return Json(name);
   }

   [HttpGet("enddate/{endDate}")]
   public ActionResult<OpenEndDate> RoundTripGet(OpenEndDate endDate)
   {
      if (!ModelState.IsValid)
         return BadRequest(ModelState);

      _logger.LogInformation("Round trip test with {Type}: {EndDate}", endDate.GetType().Name, endDate);

      return Json(endDate);
   }

   [HttpPost("enddate")]
   public ActionResult<OpenEndDate> RoundTripPost([FromBody] OpenEndDate endDate)
   {
      if (!ModelState.IsValid)
         return BadRequest(ModelState);

      _logger.LogInformation("Round trip test with {Type}: {EndDate}", endDate.GetType().Name, endDate);

      return Json(endDate);
   }

   [HttpGet("textOrNumber/{textOrNumber}")]
   public ActionResult<TextOrNumberSerializable> RoundTrip(TextOrNumberSerializable textOrNumber)
   {
      if (!ModelState.IsValid)
         return BadRequest(ModelState);

      _logger.LogInformation("Round trip test with {Type}: {EndDate}", textOrNumber.GetType().Name, textOrNumber);

      return Json(textOrNumber);
   }

   private ActionResult<T> RoundTripInternal<T>(T value)
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
