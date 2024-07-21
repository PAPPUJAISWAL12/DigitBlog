using System;
using System.Collections.Generic;

namespace DigitBlog.Models;

public partial class BlogSubscription
{
    public int SubId { get; set; }

    public decimal SubAmount { get; set; }

    public short UserId { get; set; }

    public long Bid { get; set; }

    public virtual Blog BidNavigation { get; set; } = null!;

    public virtual UserList User { get; set; } = null!;
}
