using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
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

        // Seed roles
        builder.Entity<IdentityRole>().HasData(
            new IdentityRole
            {
                Id = "1",
                Name = "Admin",
                NormalizedName = "ADMIN"
            },
            new IdentityRole
            {
                Id = "2",
                Name = "User",
                NormalizedName = "USER"
            },
            new IdentityRole
            {
                Id = "3",
                Name = "Librarian",
                NormalizedName = "LIBRARIAN"
            }
        );

        // Seed categories
        builder.Entity<Category>().HasData(
            new Category
            {
                Id = 1,
                Name = "Academic",
            },
            new Category
            {
                Id = 2,
                Name = "Science Fiction",
            },
            new Category
            {
                Id = 3,
                Name = "Adventure",
            },
            new Category
            {
                Id = 4,
                Name = "Romance",
            }
        );

        // Seed authors
        builder.Entity<Author>().HasData(
            new Author
            {
                Id = 1,
                FirstName = "Agatha",
                LastName = "Christie"
            },
            new Author
            {
                Id = 2,
                FirstName = "Mark",
                LastName = "Twain"
            }
        );
    }
}