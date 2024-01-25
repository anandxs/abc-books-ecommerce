using AbcBooks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AbcBooks.DataAccess.Repository.IRepository
{
	public interface IWalletRepository : IRepository<Wallet>
	{
		void Update(Wallet wallet);
		void UpdateBalance(string applicationUserId, float amount, bool isCredit = true);
		Wallet GetWalletWithUser(Expression<Func<Wallet, bool>> filter);
	}
}
