using DigitBlog.Models;
using DigitBlog.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Security.Claims;

namespace DigitBlog.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDataProtector _protector;
        private readonly IWebHostEnvironment _env;
        private readonly DigitalBlogContext _appContext;
        public HomeController(ILogger<HomeController>
            logger,DataSecurityKey key,
            IDataProtectionProvider provider,IWebHostEnvironment env,
            DigitalBlogContext context)
        {
            _logger = logger;
            _protector = provider.CreateProtector(key.DataKey);
            _env = env;
            _appContext = context;
        }

        [Authorize(Roles ="Admin,Editor")]
        public IActionResult Index()
        {
            return View();
        }
        
        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult AddUser()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult AddUser(UserListEdit edit)
        {
            try
            {
                short maxId;
                if (_appContext.UserLists.Any())
                    maxId = Convert.ToInt16(_appContext.UserLists.Max(u => u.UserId) + 1);
                else
                    maxId = 1;
                
                edit.UserId = maxId;
               
                if (edit.UserFile != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(edit.UserFile.FileName);
                    string imgPath = Path.Combine(_env.WebRootPath,"UserProfile",fileName);
                    using (FileStream stream =new FileStream(imgPath, FileMode.Create))
                    {
                        edit.UserFile.CopyTo(stream);
                    }
                    edit.UserProfile = fileName;
                }

                UserList u = new()
                {
                    UserId = edit.UserId,
                    UserRole = "User",
                    LoginName = edit.LoginName,
                    LoginPassword = _protector.Protect(edit.LoginPassword),
                    EmailAddress = edit.EmailAddress,
                    FullName = edit.FullName,
                    LoginStatus = true,
                    Phone = edit.Phone,
                    UserProfile = edit.UserProfile
                };
                _appContext.UserLists.Add(u);
                _appContext.SaveChanges();
                return RedirectToAction("Login","Account");
            }
            catch(Exception ex)
            {
                return View(edit);
            }
        }

        [HttpGet]
        public IActionResult ProfileImg()
        {
            var user = _appContext.UserLists.Where(u => u.UserId == Convert.ToInt16(User.Identity!.Name)).FirstOrDefault();
            ViewData["img"] = user!.UserProfile;
            return PartialView("_ProfileImg");
        }

        [HttpGet]
        public IActionResult ProfileUpdate()
        {
            var user = _appContext.UserLists.Where(u => u.UserId == Convert.ToInt16(User.Identity!.Name)).FirstOrDefault();
            ViewBag.psw = user.LoginPassword;
            if (user == null)
            {
                return NotFound();
            }
            else
            {
                UserListEdit edit = new()
                {
                    UserId = user.UserId,
                    EmailAddress = user.EmailAddress,
                    FullName = user.FullName,
                    LoginName = user.LoginName,
                    LoginPassword = user.LoginPassword,
                    UserProfile = user.UserProfile,
                    UserRole = user.UserRole,
                    LoginStatus = user.LoginStatus,
                    Phone = user.Phone
                };
                return View(edit);
            }
            
        }

        [HttpPost]
        public IActionResult ProfileUpdate(UserListEdit edit)
        {

            /*var user = _appContext.UserLists.Where(x => x.UserId == edit.UserId).FirstOrDefault();

            if (user == null)
            {
                return NotFound();
            }*/

            if (edit.UserFile != null)
                {
                    string oldFilePath = Path.Combine(_env.WebRootPath, "UserProfile", edit.UserProfile);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }

                string filename = Guid.NewGuid().ToString() + Path.GetExtension(edit.UserFile.FileName);
                    string path = Path.Combine(_env.WebRootPath, "UserProfile", filename);
                    using (FileStream str = new FileStream(path, FileMode.Create))
                    {
                        edit.UserFile.CopyTo(str);
                    }

                    edit.UserProfile = filename;
                }

                UserList u = new()
                {
                    UserId = edit.UserId,
                    UserProfile = edit.UserProfile,
                    EmailAddress = edit.EmailAddress,
                    LoginName = edit.LoginName,
                    LoginPassword = edit.LoginPassword,
                    FullName = edit.FullName,
                    LoginStatus = true,
                    Phone = edit.Phone,
                    UserRole = User.Claims.FirstOrDefault(c=>c.Type==ClaimTypes.Role).Value
                };
                
                _appContext.Update(u);
                _appContext.SaveChanges();
                return Json(u);
                
           
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
