using DigitBlog.Models;
using DigitBlog.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;
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

        [Authorize]
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePsw p)
        {
            try {
                var user = await _appContext.UserLists.Where(c => c.UserId == Convert.ToInt16(User.Identity!.Name)).FirstOrDefaultAsync();
                if (user != null)
                {
                    if (_protector.Unprotect(user.LoginPassword) == p.CurrentPassword)
                    {
                        if (p.NewPassword == p.ConfirmPassword)
                        {
                            user.LoginPassword = _protector.Protect(p.NewPassword);
                            _appContext.Update(user);
                            await _appContext.SaveChangesAsync();
                            return Json("Success");
                        }
                        else
                        {
                            ModelState.AddModelError("","Confirm Password does not matched.");
                            return View(p);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("","Please, Re-Check your Current Password?");
                        return View(p);
                    }
                }
                else
                {
                    return View();
                }
            } catch(Exception ex) 
            {
                return View(ex);
            }
        }


        [AllowAnonymous]
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ForgotPassword(UserListEdit edit)
        {
            try
            {
                var user = _appContext.UserLists.Where(u => u.EmailAddress == edit.EmailAddress).FirstOrDefault();
                if (user == null)
                {
                    ModelState.AddModelError("","This User Does not exist.");
                    return View(edit);
                }

                Random r = new Random();
                HttpContext.Session.SetString("token",r.Next(9999).ToString());
                var token = HttpContext.Session.GetString("token");

                SmtpClient smtpClient = new()
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential("jaiswalpappu873@gmail.com", "tbsd sxte srxb ublz"),//gmail_id, app Password
                    EnableSsl = true,
                    DeliveryMethod=SmtpDeliveryMethod.Network
                };

                MailMessage m = new()
                {
                    From = new MailAddress("jaiswalpappu873@gmail.com"),
                    Subject = "forgot Password",
                    Body = $"<a style='background-color:Green; color:white; padding:5px;' href='https://localhost:7092/Account/ResetPassword/{_protector.Protect(token)}'>ResetPassword</a> " +
                    $"<p>token number:{token}</p>",
                    IsBodyHtml=true
                };

                m.To.Add(user.EmailAddress);
                smtpClient.Send(m);
                return RedirectToAction("VerrifyToken");
            }
            catch(Exception ex)
            {
                return View(ex);
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult VerrifyToken()
        {
            return View();
        }


    }
}
