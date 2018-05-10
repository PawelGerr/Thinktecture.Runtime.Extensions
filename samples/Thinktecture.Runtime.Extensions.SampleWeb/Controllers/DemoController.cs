using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Thinktecture.Runtime.Extensions.Samples;
using Thinktecture.Runtime.Extensions.Samples.EnumLikeClass;

namespace Thinktecture.Runtime.Extensions.SampleWeb.Controllers
{
	[Route("api")]
	public class DemoController : Controller
	{
		private readonly ILogger<DemoController> _logger;

		public DemoController(ILogger<DemoController> logger)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		[HttpGet("{category}")]
		public IActionResult RoundTrip(ProductCategory category)
		{
			_logger.LogInformation($"Round trip test with product category {{{nameof(ProductCategory)}}}", category);

			return Json(new { ProvidedCategory = category, category.IsValid });
		}
	}
}
