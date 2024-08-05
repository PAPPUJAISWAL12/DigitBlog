using DigitBlog.Models;
using DigitBlog.Security;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            var blogList = await _appContext.Blogs.Include(u=>u.User).ToListAsync();
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
            return View(bgList);
        }


        [HttpGet]
        public IActionResult AddBlog()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddBlog(BlogEdit edit)
        {
            return View(edit);
        }


    }
}
