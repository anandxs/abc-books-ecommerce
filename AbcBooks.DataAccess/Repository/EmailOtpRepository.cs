using AbcBooks.DataAccess.Repository.IRepository;
using AbcBooks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbcBooks.DataAccess.Repository
{
	public class EmailOtpRepository : Repository<EmailOtp>, IEmailOtpRepository
	{
		private readonly ApplicationDbContext _db;

        public EmailOtpRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
