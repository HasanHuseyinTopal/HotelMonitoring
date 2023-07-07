using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Capitol.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(IdentityUser ıdentityUser)
        {
            return View();
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(IdentityUser ıdentityUser)
        {
            return View();
        }
    }
}
