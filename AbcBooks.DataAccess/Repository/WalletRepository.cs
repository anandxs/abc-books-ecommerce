using AbcBooks.DataAccess.Repository.IRepository;
using AbcBooks.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AbcBooks.DataAccess.Repository
{
	public class WalletRepository : Repository<Wallet>, IWalletRepository
	{
		private readonly ApplicationDbContext _db;

		public WalletRepository(ApplicationDbContext db) : base(db)
		{
			_db = db;
		}

		public Wallet GetWalletWithUser(Expression<Func<Wallet, bool>> filter)
		{
			return _db.Wallets
				.Include(x => x.ApplicationUser)
				.FirstOrDefault(filter)!;
		}

		public void Update(Wallet wallet)
		{
			_db.Update(wallet);
		}

		public void UpdateBalance(string applicationUserId, float amount, bool isCredit = true)
		{
			Wallet wallet = _db.Wallets.First(x => x.ApplicationUserId == applicationUserId);

			if (isCredit)
			{
				wallet.Balance += amount;
			}
			else
			{
				wallet.Balance -= amount;
			}
		}
	}
}
