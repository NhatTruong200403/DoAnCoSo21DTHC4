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
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Booking>()
                .HasOne(b => b.Invoice)
                .WithOne(i => i.Booking)
                .HasForeignKey<Invoice>(i => i.BookingId);

            // Nếu bạn cần thiết lập tự động tạo khóa chính (primary key) cho các quan hệ một-một, bạn có thể sử dụng câu lệnh dưới đây:
            builder.Entity<Booking>()
            .HasKey(b => b.Id);

            builder.Entity<Invoice>()
                .HasKey(i => i.Id);
            base.OnModelCreating(builder);
        }
    }
}
