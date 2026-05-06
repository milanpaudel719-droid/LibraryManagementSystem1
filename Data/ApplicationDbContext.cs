using System.Collections.Generic;
using LibraryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<LibraryProfile> LibraryProfiles { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<BorrowingSetting> BorrowingSettings { get; set; }
        public DbSet<BorrowTransaction> BorrowTransactions { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
    }
}