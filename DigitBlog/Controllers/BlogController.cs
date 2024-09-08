using DigitBlog.Models;
using DigitBlog.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace DigitBlog.Controllers
{
    public class BlogController : Controller
    {
       
        private readonly IDataProtector _protector;
        private readonly IWebHostEnvironment _env;
        private readonly DigitalBlogContext _appContext;
        public BlogController(DataSecurityKey key,
            IDataProtectionProvider provider, IWebHostEnvironment env,
            DigitalBlogContext context)
        {
            _appContext = context;
            _protector = provider.CreateProtector(key.DataKey);
            _env = env;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            
            return View();
        }

        public async Task<IActionResult> GetBlogList(BlogEdit edit)
        {
            var blogList = await _appContext.Blogs.Include(u => u.User).Where(b=>b.Bstatus==edit.Bstatus).ToListAsync();
            var bgList = blogList.Select(e => new BlogEdit
            {
                Bid = e.Bid,
                Amount = e.Amount,
                Bdescription = e.Bdescription,
                BlogImage = e.BlogImage,
                BlogPostDate = e.BlogPostDate,
                Bstatus = e.Bstatus,
                Title = e.Title,
                UserId = e.UserId,
                PublishedBy = e.User.FullName
            }).ToList();
            return PartialView("_GetBlogList",bgList);
        }

        [Authorize(Roles ="Admin,Editor")]
        [HttpGet]
        public IActionResult AddBlog()
        {
            return View();
        }

        [Authorize(Roles = "Admin,Editor")]
        [HttpPost]
        public IActionResult AddBlog(BlogEdit edit)
        {
            long BId = _appContext.Blogs.Any() ? _appContext.Blogs.Max(m => m.Bid) + 1 : 1;
            edit.Bid = BId;

            string filename = Guid.NewGuid().ToString() + Path.GetExtension(edit.BlogFile!.FileName);
            string path = Path.Combine(_env.WebRootPath, "Images/Blogs", filename);
            if (edit.BlogFile != null)
            {
                using (FileStream stream = new FileStream(path, FileMode.Create))
                {
                    edit.BlogFile.CopyTo(stream);
                }
                edit.BlogImage = filename;
            }

            Blog b = new()
            {
                Bid = edit.Bid,
                Amount = edit.Amount,
                Bdescription = edit.Bdescription,
                BlogImage = edit.BlogImage,
                BlogPostDate = DateOnly.FromDateTime(DateTime.Today),
                Bstatus = edit.Bstatus,
                Title = edit.Title,
                UserId = Convert.ToInt16(User.Identity!.Name)
            };
            _appContext.Add(b);
            _appContext.SaveChanges();
            return Json(b);
            try
            {
                
            }catch(Exception ex){
                return Json(ex);
            }
            return View(edit);
        }

        [Authorize(Roles ="Admin")]
        public IActionResult Edit(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var blogs = _appContext.Blogs.Where(b=>b.Bid==id).Include(b=>b.User).First();
            if (blogs != null)
            {
                BlogEdit e = new()
                {
                    Bid = blogs.Bid,
                    Title = blogs.Title,
                    Amount = blogs.Amount,
                    Bdescription = blogs.Bdescription,
                    BlogImage = blogs.BlogImage,
                    BlogPostDate = blogs.BlogPostDate,
                    Bstatus = blogs.Bstatus,
                    UserId = blogs.UserId,
                    PublishedBy = blogs.User.FullName
                };
             
                return View(e);
            }
            else
            {
                return Content("try again!.");
            }
          
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Edit(BlogEdit edit)
        {
            
           
            if (edit.BlogFile != null)
            {
                string filename = Guid.NewGuid().ToString() + Path.GetExtension(edit.BlogFile!.FileName);
                string path = Path.Combine(_env.WebRootPath, "Images/Blogs", filename);
                using (FileStream stream = new FileStream(path, FileMode.Create))
                {
                    edit.BlogFile.CopyTo(stream);
                }
                edit.BlogImage = filename;
            }

            Blog b = new()
            {
                Bid = edit.Bid,
                Amount = edit.Amount,
                Bdescription = edit.Bdescription,
                BlogImage = edit.BlogImage,
                BlogPostDate = DateOnly.FromDateTime(DateTime.Today),
                Bstatus = edit.Bstatus,
                Title = edit.Title,
                UserId = Convert.ToInt16(User.Identity!.Name)
            };
            _appContext.Update(b);
            _appContext.SaveChanges();
            return Content("success");
            try
            {

            }
            catch (Exception ex)
            {
                return Json(ex);
            }
            return View(edit);
        }


        [Authorize(Roles ="Admin")]
        public IActionResult Delete(int id)
        {
            var blogs = _appContext.Blogs.Where(x => x.Bid == id).First();
            if (blogs != null)
            {
                _appContext.Remove(blogs);
                _appContext.SaveChanges();
                return Content("success");
            }
            else
            {
                return Content("Failed");
            }
        }

        public IActionResult Success(string q, string oid, string amt, string refId)
        {
            return Json(oid);
            Blog? sub = _appContext.Blogs.Where(x => x.Bid == Convert.ToInt32(oid)).FirstOrDefault();
            if (sub != null)
            {
                string msg = "Payment Successful. Rs. " + amt;
                return View((object)msg);
            }
            return View();
            
        }
        public IActionResult Failure()
        {
            return View();
        }

    }
}
