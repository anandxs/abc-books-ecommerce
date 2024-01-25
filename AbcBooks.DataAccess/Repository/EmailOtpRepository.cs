using AbcBooks.DataAccess.Repository.IRepository;
using AbcBooks.Models;

namespace AbcBooks.DataAccess.Repository;

public class EmailOtpRepository : Repository<EmailOtp>, IEmailOtpRepository
{
    private readonly ApplicationDbContext _db;

    public EmailOtpRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }
}
