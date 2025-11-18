using Microsoft.EntityFrameworkCore;
using OnionTemplate.Application.Interfaces;
using OnionTemplate.Core.Entities;
using OnionTemplate.Core.Enums;
using OnionTemplate.Infrastructure.Data;

namespace OnionTemplate.Infrastructure.Repositories
{
    public class AddressRepository : Repository<Address>, IAddressRepository
    {
        public AddressRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Address>> GetAddressesByUserAsync(Guid userId)
        {
            return await _dbSet
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.IsDefault)
                .ThenByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<Address?> GetDefaultAddressAsync(Guid userId, AddressType type)
        {
            return await _dbSet
                .FirstOrDefaultAsync(a => a.UserId == userId && 
                                        a.IsDefault && 
                                        (a.Type == type || a.Type == AddressType.Both));
        }

        public async Task SetDefaultAddressAsync(Guid userId, Guid addressId, AddressType type)
        {
            // Remove default flag from other addresses of the same type
            var existingDefaults = await _dbSet
                .Where(a => a.UserId == userId && 
                           a.IsDefault && 
                           (a.Type == type || a.Type == AddressType.Both))
                .ToListAsync();

            foreach (var address in existingDefaults)
            {
                address.IsDefault = false;
            }

            // Set the new default
            var newDefault = await _dbSet.FindAsync(addressId);
            if (newDefault != null && newDefault.UserId == userId)
            {
                newDefault.IsDefault = true;
                newDefault.Type = type;
            }
        }
    }
}

