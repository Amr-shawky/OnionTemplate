using OnionTemplate.Core.Entities;
using OnionTemplate.Core.Enums;

namespace OnionTemplate.Application.Interfaces
{
    public interface IAddressRepository : IRepository<Address>
    {
        Task<IEnumerable<Address>> GetAddressesByUserAsync(Guid userId);
        Task<Address?> GetDefaultAddressAsync(Guid userId, AddressType type);
        Task SetDefaultAddressAsync(Guid userId, Guid addressId, AddressType type);
    }
}

