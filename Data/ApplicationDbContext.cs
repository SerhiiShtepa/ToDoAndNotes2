using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ToDoAndNotes2.Models;

namespace ToDoAndNotes2.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Folder> Folders { get; set; }
        public DbSet<Models.Task> Tasks { get; set; }
        public DbSet<Note> Notes { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder
            //     .EnableDetailedErrors()
            //     .EnableSensitiveDataLogging()
            //     .LogTo(Console.WriteLine, LogLevel.Information);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Folder>()
                .HasMany(t => t.Tasks)
                .WithOne(f => f.Folder)
                .HasPrincipalKey(f => f.FolderId)
                .HasForeignKey(f => f.FolderId);
            modelBuilder.Entity<Folder>()
                .HasMany(n => n.Notes)
                .WithOne(f => f.Folder)
                .HasPrincipalKey(f => f.FolderId)
                .HasForeignKey(n => n.FolderId);
            base.OnModelCreating(modelBuilder);
        }
    }
}