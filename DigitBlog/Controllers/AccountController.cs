using DigitBlog.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DigitBlog.Controllers
{
    public class AccountController : Controller
    {
        private readonly DigitalBlogContext _appContext;
        public AccountController(DigitalBlogContext dbcontext)
        {
            _appContext = dbcontext;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(UserListEdit edit)
        {
            var u = await _appContext.UserLists.ToListAsync();
            if (u != null)
            {
                var user = u.Where(e=>
                (e.LoginName.ToUpper().Equals
                (edit.LoginName.ToUpper()) || e.EmailAddress.ToUpper().Equals(edit.EmailAddress.ToUpper()))
                && e.LoginPassword.Equals(edit.LoginPassword)
                && e.LoginStatus==true).FirstOrDefault();
                if (user != null)
                {
                    List<Claim> claim = new()
                    {
                        new Claim(ClaimTypes.Name,user.UserId.ToString()),
                        new Claim(ClaimTypes.Role,user.UserRole),
                        new Claim(ClaimTypes.Email,user.EmailAddress),
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
            return Json("Hello Dashboard");
        }

    }
}
