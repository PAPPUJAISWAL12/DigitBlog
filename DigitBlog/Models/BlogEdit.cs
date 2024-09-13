using System.ComponentModel.DataAnnotations;

namespace DigitBlog.Models
{
    public class BlogEdit
    {

        public long Bid { get; set; }

        public string Title { get; set; } = null!;

        public string Bdescription { get; set; } = null!;

        public string? BlogImage { get; set; }

        [DataType(DataType.Upload)]
        public IFormFile? BlogFile { get; set; }

        public DateOnly BlogPostDate { get; set; }

        public short UserId { get; set; }
        public string? PublishedBy { get; set; }

        public string Bstatus { get; set; } = null!;

        public decimal Amount { get; set; }
        public string BlogEncId { get; set; }
    }
}
