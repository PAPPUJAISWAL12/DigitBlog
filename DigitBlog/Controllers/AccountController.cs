using DigitBlog.Models;
using DigitBlog.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DigitBlog.Controllers
{
    public class AccountController : Controller
    {
        private readonly DigitalBlogContext _appContext;
        private readonly IDataProtector _protector;
        public AccountController(DigitalBlogContext dbcontext,DataSecurityKey key,IDataProtectionProvider provider)
        {
            _appContext = dbcontext;
            _protector = provider.CreateProtector(key.DataKey);
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("Index","Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserListEdit edit)
        {
            var u = await _appContext.UserLists.ToListAsync();
            if (u != null)
            {
                var user = u.Where(e => e.LoginName.ToUpper().Equals(edit.LoginName.ToUpper()) && _protector.Unprotect(e.LoginPassword).Equals(edit.LoginPassword)
                && e.LoginStatus == true).FirstOrDefault();
                if (user != null)
                {
                    List<Claim> claim = new()
                    {
                        new Claim(ClaimTypes.Name,user.UserId.ToString()),
                        new Claim(ClaimTypes.Role,user.UserRole),
                        new Claim("Email",user.EmailAddress),
                        new Claim("FullName",user.FullName)
                    };
                    var identity = new ClaimsIdentity(claim, CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity),new AuthenticationProperties { IsPersistent=edit.RememberMe});
                    return RedirectToAction("Dashboard");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid UserName or Password");
                    return View(edit);
                }
            }
            else
            {
                ModelState.AddModelError("","this user does not exist.");
                return View(edit);
            }
        }

        [Authorize]
        public IActionResult Dashboard()
        {
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("Index","Home");
            }else if (User.IsInRole("Editor"))
            {
                return RedirectToAction("Index","Home");
            }
            else
            {
                return RedirectToAction("Privacy","Home");
            }
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index","Static");
        }

    }
}
