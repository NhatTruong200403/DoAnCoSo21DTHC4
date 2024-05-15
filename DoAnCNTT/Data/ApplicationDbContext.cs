using DoAnCNTT.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace DoAnCNTT.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Amenity> Amenities { get; set; }
        public DbSet<CarType> CarTypes { get; set; }
        public DbSet<Booking> Booking { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostImages> PostImages { get; set; }   
        public DbSet<PostAmenity> PostAmenities { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<Rating> Ratings { get; set; }   
        public DbSet<Company> Companies { get; set; }
        public DbSet<CarTypeDetail> CarTypesDetails { get; set; }
        public DbSet<Favorite> Favorite { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Invoice>()
                .HasKey(i => i.Id);
            base.OnModelCreating(builder);

            builder.Entity<Favorite>()
           .HasOne(f => f.User)
           .WithMany(u => u.Favorites)
           .HasForeignKey(f => f.UserId);

            builder.Entity<Favorite>()
                .HasOne(f => f.Post)
                .WithMany(p => p.Favorites)
                .HasForeignKey(f => f.PostId);
        }
    }
}
