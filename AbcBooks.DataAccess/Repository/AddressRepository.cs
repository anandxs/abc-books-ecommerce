using AbcBooks.DataAccess.Repository.IRepository;
using AbcBooks.Models;

namespace AbcBooks.DataAccess.Repository;

public class AddressRepository : Repository<Address>, IAddressRepository
{
    private readonly ApplicationDbContext _db;

    public AddressRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public IEnumerable<Address> GetAllAddressesByUser(string applicationUserId)
    {
        return _db.Addresses.Where(x => x.ApplicationUserId == applicationUserId && !x.IsDeleted);
    }

    public void Update(Address address)
    {
        _db.Addresses.Update(address);
    }

    public void MakeAddressDefault(int id)
    {
        Address address = _db.Addresses.First(x => x.Id == id);
        address.IsDefault = true;

        foreach (var x in _db.Addresses.Where(x => x.ApplicationUserId == address.ApplicationUserId && !x.IsDeleted))
        {
            if (!x.Equals(address))
            {
                x.IsDefault = false;
            }
        }
    }

    public void Delete(Address address)
    {
        address.IsDeleted = true;
    }
}
