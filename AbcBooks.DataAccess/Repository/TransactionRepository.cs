using AbcBooks.DataAccess.Repository.IRepository;
using AbcBooks.Models;

namespace AbcBooks.DataAccess.Repository;

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
