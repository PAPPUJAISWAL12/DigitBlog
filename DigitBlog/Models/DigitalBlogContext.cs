using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DigitBlog.Models;

public partial class DigitalBlogContext : DbContext
{
    public DigitalBlogContext()
    {
    }

    public DigitalBlogContext(DbContextOptions<DigitalBlogContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Blog> Blogs { get; set; }

    public virtual DbSet<BlogSubscription> BlogSubscriptions { get; set; }

    public virtual DbSet<UserList> UserLists { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("name=Conn");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Blog>(entity =>
        {
            entity.HasKey(e => e.Bid).HasName("PK__Blog__C6DE0CC1266F9AAC");

            entity.ToTable("Blog");

            entity.Property(e => e.Bid)
                .ValueGeneratedNever()
                .HasColumnName("BId");
            entity.Property(e => e.Amount).HasColumnType("decimal(8, 2)");
            entity.Property(e => e.Bdescription).HasColumnName("BDescription");
            entity.Property(e => e.Bstatus)
                .HasMaxLength(50)
                .HasColumnName("BStatus");

            entity.HasOne(d => d.User).WithMany(p => p.Blogs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Blog__UserId__4F7CD00D");
        });

        modelBuilder.Entity<BlogSubscription>(entity =>
        {
            entity.HasKey(e => e.SubId).HasName("PK__BlogSubs__4D9BB84A21EA1087");

            entity.ToTable("BlogSubscription");

            entity.Property(e => e.SubId).ValueGeneratedNever();
            entity.Property(e => e.Bid).HasColumnName("BId");
            entity.Property(e => e.SubAmount).HasColumnType("decimal(8, 2)");

            entity.HasOne(d => d.BidNavigation).WithMany(p => p.BlogSubscriptions)
                .HasForeignKey(d => d.Bid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BlogSubscri__BId__5535A963");

            entity.HasOne(d => d.User).WithMany(p => p.BlogSubscriptions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BlogSubsc__UserI__5441852A");
        });

        modelBuilder.Entity<UserList>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__UserList__1788CC4CFB665371");

            entity.ToTable("UserList");

            entity.HasIndex(e => e.EmailAddress, "UQ__UserList__49A14740D4C033EB").IsUnique();

            entity.HasIndex(e => e.Phone, "UQ__UserList__5C7E359ED2A28E0C").IsUnique();

            entity.Property(e => e.UserId).ValueGeneratedNever();
            entity.Property(e => e.EmailAddress).HasMaxLength(40);
            entity.Property(e => e.FullName).HasMaxLength(50);
            entity.Property(e => e.LoginName).HasMaxLength(30);
            entity.Property(e => e.LoginStatus).HasDefaultValue(true);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.UserRole).HasMaxLength(30);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
