using AbcBooks.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AbcBooks.DataAccess
{
    public class ApplicationDbContext : IdentityDbContext
	{
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        { }

        public DbSet<Category> Categories { get; set; }
		public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<ShoppingCart> ShoppingCart { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<EmailOtp> EmailOtps { get; set; }
		public DbSet<Wallet> Wallets { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<DiscountType> DiscountTypes { get; set; }
        public DbSet<UserCoupon> UserCoupons { get; set; }
        public DbSet<Wishlist> Wishlists { get; set; }
		public DbSet<Banner> Banners { get; set; }
        public DbSet<Referral> Referrals { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<Category>()
				.Property(x => x.Name)
				.IsRequired();

			modelBuilder.Entity<Order>()
				.HasOne(x => x.ShippingAddress)
				.WithMany()
				.OnDelete(DeleteBehavior.NoAction);

			modelBuilder.Entity<Order>()
				.HasOne(x => x.ApplicationUser)
				.WithMany()
				.OnDelete(DeleteBehavior.NoAction);

			modelBuilder.Entity<Referral>()
				.Property(x => x.IsFulfilled)
				.HasDefaultValue(false);
		}
	}
}
