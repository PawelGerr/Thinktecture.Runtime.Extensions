using Microsoft.AspNetCore.Mvc;
using Thinktecture.Runtime.Extensions.Samples;

namespace Thinktecture.Runtime.Extensions.SampleWeb.Controllers
{
	[Route("api")]
	public class DemoController : Controller
	{
		[HttpGet("{category}")]
		public IActionResult RoundTrip(ProductCategory category)
		{
			return Json(new { ProvidedCategory = category, category.IsValid });
		}
	}
}
