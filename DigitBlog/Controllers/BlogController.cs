﻿using DigitBlog.Models;
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


    }
}