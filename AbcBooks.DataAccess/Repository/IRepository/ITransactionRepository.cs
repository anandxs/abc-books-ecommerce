using AbcBooks.Models;

namespace AbcBooks.DataAccess.Repository.IRepository;

public interface ITransactionRepository : IRepository<Transaction>
{
    IEnumerable<Transaction> GetAllTransactionForUser(int walletId);
}
