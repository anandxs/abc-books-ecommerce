using AbcBooks.DataAccess.Repository.IRepository;
using AbcBooks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbcBooks.DataAccess.Repository
{
	public class TransactionRepository : Repository<Transaction>, ITransactionRepository
	{
		private readonly ApplicationDbContext _db;

		public TransactionRepository(ApplicationDbContext db) : base(db)
		{
			_db = db;
		}

		public IEnumerable<Transaction> GetAllTransactionForUser(int walletId)
		{
			return _db.Transactions.Where(x => x.WalletId == walletId);
		}
	}
}
