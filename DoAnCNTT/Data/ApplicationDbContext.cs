using DoAnCNTT.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

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
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostAmenity> PostAmenities { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<Rating> Ratings { get; set; }   
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
