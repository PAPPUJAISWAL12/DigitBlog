using System;
using System.Collections.Generic;

namespace DigitBlog.Models;

public partial class UserList
{
    public short UserId { get; set; }

    public string LoginName { get; set; } = null!;

    public string LoginPassword { get; set; } = null!;

    public string UserProfile { get; set; } = null!;

    public bool LoginStatus { get; set; }

    public string UserRole { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string EmailAddress { get; set; } = null!;

    public string? Phone { get; set; }

    public virtual ICollection<BlogSubscription> BlogSubscriptions { get; set; } = new List<BlogSubscription>();

    public virtual ICollection<Blog> Blogs { get; set; } = new List<Blog>();
}
