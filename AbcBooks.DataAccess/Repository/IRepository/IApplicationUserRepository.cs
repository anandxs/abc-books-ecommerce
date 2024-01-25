using AbcBooks.Models;

namespace AbcBooks.DataAccess.Repository.IRepository;

public interface IApplicationUserRepository : IRepository<ApplicationUser>
{
    void Update(ApplicationUser applicationUser);
}
