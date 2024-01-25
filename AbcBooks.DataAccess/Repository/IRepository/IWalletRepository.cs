using AbcBooks.Models;
using System.Linq.Expressions;

namespace AbcBooks.DataAccess.Repository.IRepository;

public interface IWalletRepository : IRepository<Wallet>
{
    void Update(Wallet wallet);
    void UpdateBalance(string applicationUserId, float amount, bool isCredit = true);
    Wallet GetWalletWithUser(Expression<Func<Wallet, bool>> filter);
}
