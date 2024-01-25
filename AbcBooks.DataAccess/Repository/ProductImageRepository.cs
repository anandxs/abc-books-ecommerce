using AbcBooks.DataAccess.Repository.IRepository;
using AbcBooks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbcBooks.DataAccess.Repository
{
	public class ProductImageRepository : Repository<ProductImage>, IProductImageRepository
	{
        private readonly ApplicationDbContext _db;
        public ProductImageRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

		public IEnumerable<ProductImage> GetAllProductImages(int id)
		{
			return _db.ProductImages.Where(x => x.ProductId == id);
		}

        public int GetImageCountForProduct(int productId)
        {
            return _db.ProductImages.Count(x => x.ProductId == productId);
        }

        public void Update(ProductImage productImage)
		{
			_db.ProductImages.Update(productImage);
		}
	}
}
