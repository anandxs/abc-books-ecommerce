using AbcBooks.Models;

namespace AbcBooks.DataAccess.Repository.IRepository;

public interface IAddressRepository : IRepository<Address>
{
    void Update(Address address);
    IEnumerable<Address> GetAllAddressesByUser(string applicationUserId);
    void MakeAddressDefault(int id);
    void Delete(Address address);
}
