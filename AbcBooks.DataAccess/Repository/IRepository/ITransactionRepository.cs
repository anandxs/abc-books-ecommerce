using AbcBooks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbcBooks.DataAccess.Repository.IRepository
{
	public interface ITransactionRepository : IRepository<Transaction>
	{
		IEnumerable<Transaction> GetAllTransactionForUser(int walletId);
	}
}
