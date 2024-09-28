using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using URLShortener.Enums;
using URLShortener.Models;

namespace URLShortener.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationContext _context;

        public UserController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterAsync(string login, string password, string repeatPassword)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Data is not valid.";
                return View();
            }

            if (!password.Equals(repeatPassword))
            {
                ViewBag.Error = "Passwords must match.";
                return View();
            }

            var userWithSameLogin = _context.Users.FirstOrDefault(u => u.Login == login);


            if (userWithSameLogin != null)
            {
                ViewBag.Error = "User with same login already exists.";
                return View();
            }

            var user = new User(login, password);

            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            await AuthentificateUserAsync(user);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoginAsync(string login, string password)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Data is not valid.";
                return View();
            }

            var user = _context.Users.Where(u => u.Login == login).FirstOrDefault();

            if (user == null)
            {
                ViewBag.Error = "User with this login was not found.";
                return View();
            }

            if (user.Password != password)
            {
                ViewBag.Error = "Wrong password.";
                return View();
            }

            await AuthentificateUserAsync(user);

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public async Task<IActionResult> LogoutAsync()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");
        }

        private async Task AuthentificateUserAsync(User user)
        {
            var claims = new List<Claim>
            {
                new Claim( ClaimTypes.Name, user.Login),
                new Claim( ClaimTypes.Role, ((RolesEnum) user.RoleId).ToString())
            };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme,
                ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
        }
    }
}
