using AbcBooks.DataAccess.Repository.IRepository;
using AbcBooks.Models;
using Microsoft.EntityFrameworkCore;

namespace AbcBooks.DataAccess.Repository;

public class ProductRepository : Repository<Product>, IProductRepository
{
    private readonly ApplicationDbContext _db;

    public ProductRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public IEnumerable<Product> GetAllListedProductsWithCategory()
    {
        return _db.Products
            .Where(x => x.IsListed && !x.IsDeleted)
            .Include(x => x.Category);
    }

    public IEnumerable<Product> GetAllProductsWithCategory()
    {
        return _db.Products
            .Where(x => !x.IsDeleted)
            .Include(x => x.Category);
    }

    public Product GetProductWithCategory(int id)
    {
        return _db.Products
            .Where(x => !x.IsDeleted)
            .Include(x => x.Category)
            .FirstOrDefault(x => x.Id == id)!;
    }

    public Product GetListedProductWithCategory(int id)
    {
        return _db.Products
            .Include(x => x.Category)
            .Where(x => x.IsListed && !x.IsDeleted)
            .FirstOrDefault(x => x.Id == id)!;
    }

    public void Update(Product product)
    {
        _db.Products.Update(product);
    }

    public void UpdateListingStatus(bool status, int categoryId)
    {
        var products = _db.Products
            .Where(x => x.CategoryId == categoryId);

        foreach (var product in products)
        {
            product.IsListed = status;
        }
    }

    public void UpdateStock(int productId, int decrement)
    {
        Product product = _db.Products.First(x => x.Id == productId);
        product.Stock -= decrement;
    }

    public IEnumerable<Product> GetAllProductsOfCategory(Category category)
    {
        return _db.Products.Where(x => x.CategoryId == category.Id);
    }

    public void UpdateDiscount(int productId, float discount)
    {
        Product product = _db.Products.First(x => x.Id == productId);
        product.Discount = discount;
    }

    public void Delete(Product product)
    {
        product.IsListed = false;
        product.IsDeleted = true;
    }

    public IEnumerable<Product> GetBestSellers()
    {
        var orderDetails = _db.OrderDetails.Include(x => x.Product).ToList();

        Dictionary<int, int> productCount = new();

        foreach (var item in orderDetails)
        {
            if (!item.Product.IsListed)
                continue;

            if (productCount.ContainsKey(item.ProductId))
            {
                productCount[item.ProductId]++;
            }
            else
            {
                productCount.Add(item.ProductId, item.Quantity);
            }
        }

        int count = 0;
        IEnumerable<Product> bestSellers =
            productCount.OrderByDescending(x => x.Value)
                .Take(4)
                .Select(x =>
                {
                    count++;
                    return _db.Products.First(y => y.Id == x.Key);
                });

        return bestSellers;
    }
}
