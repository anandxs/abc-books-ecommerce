using AbcBooks.DataAccess.Repository.IRepository;

namespace AbcBooks.DataAccess.Repository;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _db;

    public ICategoryRepository Categories { get; private set; }
    public IProductRepository Products { get; private set; }
    public IApplicationUserRepository ApplicationUsers { get; private set; }
    public IShoppingCartRepository ShoppingCart { get; private set; }
    public IAddressRepository Addresses { get; private set; }
    public IOrderRepository Orders { get; private set; }
    public IOrderDetailRepository OrderDetails { get; private set; }
    public IEmailOtpRepository EmailOtps { get; private set; }
    public IProductImageRepository ProductImages { get; set; }
    public IWalletRepository Wallets { get; set; }
    public ITransactionRepository Transactions { get; set; }
    public ICouponRepository Coupons { get; set; }
    public IDiscountTypeRepository DiscountTypes { get; set; }
    public IUserCouponRepository UserCoupons { get; set; }
    public IWishlistRepository Wishlist { get; set; }
    public IBannerRepository Banner { get; set; }
    public IReferralRepository Referral { get; set; }

    public UnitOfWork(ApplicationDbContext db)
    {
        _db = db;
        Categories = new CategoryRepository(_db);
        Products = new ProductRepository(_db);
        ApplicationUsers = new ApplicationUserRepository(_db);
        ShoppingCart = new ShoppingCartRepository(_db);
        Addresses = new AddressRepository(_db);
        Orders = new OrderRepository(_db);
        OrderDetails = new OrderDetailRepository(_db);
        EmailOtps = new EmailOtpRepository(_db);
        ProductImages = new ProductImageRepository(_db);
        Wallets = new WalletRepository(_db);
        Transactions = new TransactionRepository(_db);
        Coupons = new CouponRepository(_db);
        DiscountTypes = new DiscountTypeRepository(_db);
        UserCoupons = new UserCouponRepository(_db);
        Wishlist = new WishlistRepository(_db);
        Banner = new BannerRepository(_db);
        Referral = new ReferralRepository(_db);
    }

    public void Save()
    {
        _db.SaveChanges();
    }
}
