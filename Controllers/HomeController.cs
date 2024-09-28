using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using URLShortener.Models;
using URLShortener.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System;


namespace URLShortener.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationContext _context;

        public HomeController(ApplicationContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult Info([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }

            var url = _context.URLs.First(u => u.Id == id);

            return View(url);
        }

        public IActionResult About()
        {
            var text = _context.Texts.FirstOrDefault(t => t.PageName == "About")?.Value;

            return View("About", text);
        }

        [Authorize(Roles = nameof(RolesEnum.Admin))]
        public async Task<IActionResult> SaveTextAsync(string text)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("About");
            }

            var oldText = _context.Texts.Where(t => t.PageName == "About").FirstOrDefault();

            if (oldText == null)
            {
                _context.Texts.Add(new Text { PageName = "About", Value = text });
            }

            else
            {
                oldText.Value = text;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("About");
        }
                                
        [Route("URLs")]
        public JsonResult GetURLs()
        {
            var URLs = _context.URLs.ToList();

            return Json(URLs);
        }

        [Route("isAuthenticated")]
        public JsonResult IsAuthenticated()
        {
            var isAuthenticated = User.Identity?.IsAuthenticated ?? false;

            return Json(isAuthenticated);
        }

        [Authorize]
        [Route("addURL")]
        public async Task<JsonResult> AddURL([FromBody] string url)
        {
            if (!ModelState.IsValid)
            {
                return Json("Data is not valid.");
            }

            var existingLongURL = _context.URLs.FirstOrDefault(u => u.LongURL.ToLower() == url.ToLower());

            if (existingLongURL != null)
            {
                return Json("This URL already exists.");
            }

            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";

            var shortURL = $"{baseUrl}/short/{RandomString()}";

            var creatorLogin = User.Identity?.Name;
            var creatorId = _context.Users.First(u => u.Login == creatorLogin).Id;

            if (url.StartsWith("http://"))
            {
                url = url.Replace("http://", "");
            }
            else if (url.StartsWith("https://"))
            {
                url = url.Replace("https://", "");
            }

            var newUrl = new URL(url, shortURL, creatorId);

            _context.URLs.Add(newUrl);
            await _context.SaveChangesAsync();

            return Json("Created");
        }

        [Authorize]
        [Route("deleteURL")]
        public async Task<JsonResult> DeleteURL([FromBody] int id)
        {
            if (!ModelState.IsValid)
            {
                return Json("Data is not valid.");
            }

            var urlToDelete = _context.URLs.Include(u => u.Creator).FirstOrDefault(u => u.Id == id);

            if (urlToDelete == null)
            {
                return Json("This URL wasnt found.");
            }

            if (!User.IsInRole(RolesEnum.Admin.ToString()) && urlToDelete.Creator?.Login != User.Identity?.Name)
            {
                return Json("No access.");
            }

            _context.URLs.Remove(urlToDelete);

            await _context.SaveChangesAsync();

            return Json("Deleted");
        }

        private static string RandomString()
        {
            const int length = 6;

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            Random random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                                        .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        [Authorize]
        [Route("id")]
        public JsonResult GetUserId()
        {
            var login = User.Identity?.Name ?? "";

            var userId = _context.Users.FirstOrDefault(u => u.Login == login)?.Id;

            return Json(userId);
        }

        [Authorize]
        [Route("isAdmin")]
        public JsonResult IsUserAdmin()
        {
            return Json(User.IsInRole(RolesEnum.Admin.ToString()));
        }

        [Route("short/{shortUrl}")]
        public IActionResult RedirectToLong(string shortUrl)
        {
            if (!ModelState.IsValid)
            {
                return Json("Data is not valid.");
            }

            var longUrl = _context.URLs.FirstOrDefault(u => u.ShortURL.Contains(shortUrl))?.LongURL;

            return longUrl == null ? RedirectToAction("Index") : Redirect($"http://{longUrl}");
        }

    }
}
