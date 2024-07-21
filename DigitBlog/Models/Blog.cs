using System;
using System.Collections.Generic;

namespace DigitBlog.Models;

public partial class Blog
{
    public long Bid { get; set; }

    public string Title { get; set; } = null!;

    public string Bdescription { get; set; } = null!;

    public string? BlogImage { get; set; }

    public DateOnly BlogPostDate { get; set; }

    public short UserId { get; set; }

    public string Bstatus { get; set; } = null!;

    public decimal Amount { get; set; }

    public virtual ICollection<BlogSubscription> BlogSubscriptions { get; set; } = new List<BlogSubscription>();

    public virtual UserList User { get; set; } = null!;
}
