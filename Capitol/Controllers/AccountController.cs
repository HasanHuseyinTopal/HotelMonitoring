using EntityLayer.DTOs;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Capitol.Controllers
{
    public class AccountController : Controller
    {
        private SignInManager<IdentityUser> _signInManager;
        private UserManager<IdentityUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        private IConfiguration _configuration;
        public AccountController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration config)
        {
            _configuration = config;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;

        }
        [HttpGet]
        public async Task<IActionResult> Login(string? ReturnUrl)
        {
            if (ReturnUrl != null)
            {
                ViewBag.ReturnUrl = ReturnUrl;
            }
            if (_userManager.Users.Count() < 1)
            {
                string userName = _configuration["FirstUser:UserName"]!;
                string password = _configuration["FirstUser:Password"]!;
                var user = new IdentityUser()
                {
                    UserName = userName,
                };
                var result = await _userManager.CreateAsync(user, password);
                var startingRole = await _roleManager.RoleExistsAsync("ADMN");
                IdentityRole role = null!;
                if (!startingRole)
                {
                    role = new IdentityRole()
                    {
                        Name = "ADMN"
                    };
                    await _roleManager.CreateAsync(role);
                }
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, role?.Name!);
                    return View();
                }
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginDTO login, string? ReturnUrl)
        {
            if (ModelState.IsValid)
            {
                SignInResult result = null;
                var userName = await _userManager.FindByNameAsync(login.UserName);
                if (userName != null)
                {
                    var user = await _userManager.CheckPasswordAsync(userName, login.Password);
                    if (user)
                    {
                        result = await _signInManager.PasswordSignInAsync(userName, login.Password, true, false);

                    }
                    if (result.Succeeded)
                    {
                        if (ReturnUrl != null)
                            return Redirect(ReturnUrl);
                        return RedirectToAction("DailyVisitors", "Visitor");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Kullanıcı Adı veya Şifre Hatalı");
                }
                return View();
            }
            else
            {
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("DailyVisitors", "Visitor");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(LoginDTO login)
        {
            return View();
        }
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
