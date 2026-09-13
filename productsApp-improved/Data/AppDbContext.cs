using Microsoft.EntityFrameworkCore;
using productsApp_improved.Models;
// add to admin panel
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace productsApp_improved.Data
{
    //public class AppDbContext : DbContext
    public class AppDbContext : IdentityDbContext
    {
         public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
         {
         }
         public DbSet<AppUser> Users { get; set; }
         public DbSet<Product> Products { get; set; }

         public DbSet<Category> Categories { get; set; }
         public DbSet<Order> Orders { get; set; }
         public DbSet<OrderDetails> OrderDetails { get; set; }
         public DbSet<NotificationModel> Notifications { get; set; }
         public DbSet<PushSubscriptionEntity> PushSubscriptions { get; set; }
         public DbSet<AuditLog> AuditLogs { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    
        //    modelBuilder.Entity<Category>().HasData(new Category { Id = 1, Name = "Laptops"});
        //    modelBuilder.Entity<Category>().HasData(new Category { Id = 2, Name = "Cameras"});
        //    modelBuilder.Entity<Category>().HasData(new Category { Id = 3, Name = "Keyboards"});
        //}

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Product>()
        //        .HasOne(p => p.Category)
        //        .WithMany(c => c.Products)
        //        .HasForeignKey(p => p.CategoryId)
        //        // This prevents deleting the category if products exist
        //        .OnDelete(DeleteBehavior.Restrict);
        //}

         protected override void OnModelCreating(ModelBuilder modelBuilder)
         {
             base.OnModelCreating(modelBuilder);

             modelBuilder.Entity<Order>()
                 .HasMany(o => o.Products)
                 .WithMany(p => p.Orders)
                 .UsingEntity(
                     "OrderProduct", 
                     l => l.HasOne(typeof(Product)).WithMany().HasForeignKey("ProductId").OnDelete(DeleteBehavior.Restrict),
                     r => r.HasOne(typeof(Order)).WithMany().HasForeignKey("OrderId").OnDelete(DeleteBehavior.Cascade)
                 );
         }
    }
}
