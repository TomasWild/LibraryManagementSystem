using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
{
    public DbSet<LibraryCard> LibraryCards { get; set; }
    public DbSet<Member> Members { get; set; }
    public DbSet<Book> Books { get; set; }
    public DbSet<Author> Authors { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<BookCategory> BookCategories { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // one-to-one relation between Member and LibraryCard
        builder.Entity<Member>()
            .HasOne(m => m.LibraryCard)
            .WithOne(lc => lc.Member)
            .HasForeignKey<LibraryCard>(lc => lc.MemberId);

        // one-to-many relation between Author and Book
        builder.Entity<Author>()
            .HasMany(a => a.Books)
            .WithOne(b => b.Author)
            .HasForeignKey(b => b.AuthorId);

        // many-to-many relation between Book and Category
        builder.Entity<BookCategory>()
            .HasKey(c => new { c.CategoryId, c.BookId });

        builder.Entity<BookCategory>()
            .HasOne(bc => bc.Category)
            .WithMany(c => c.BookCategories)
            .HasForeignKey(bc => bc.CategoryId);

        builder.Entity<BookCategory>()
            .HasOne(bc => bc.Book)
            .WithMany(b => b.BookCategories)
            .HasForeignKey(bc => bc.BookId);
    }
}