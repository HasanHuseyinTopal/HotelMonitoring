using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Capitol.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error")]
        public IActionResult ErrorHandler()
        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            return View("ErrorHandler", context?.Error.Message);
        }
    }
}
