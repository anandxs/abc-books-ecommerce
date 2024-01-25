namespace AbcBooks.DataAccess.Repository.IRepository
{
	public interface IUnitOfWork
    {
        ICategoryRepository Categories { get; }
        IProductRepository Products { get; }
        IApplicationUserRepository ApplicationUsers { get; }
        IShoppingCartRepository ShoppingCart { get; }
        IAddressRepository Addresses { get; }
        IOrderRepository Orders { get; }
        IOrderDetailRepository OrderDetails { get; }
        IEmailOtpRepository EmailOtps { get; }
        IProductImageRepository ProductImages { get; }
        IWalletRepository Wallets { get; }
        ITransactionRepository Transactions { get; }
        ICouponRepository Coupons { get; }
        IDiscountTypeRepository DiscountTypes { get; }
        IUserCouponRepository UserCoupons { get; }
        IWishlistRepository Wishlist { get; }
        IBannerRepository Banner { get; }
        IReferralRepository Referral { get; }
        void Save();
    }
}
