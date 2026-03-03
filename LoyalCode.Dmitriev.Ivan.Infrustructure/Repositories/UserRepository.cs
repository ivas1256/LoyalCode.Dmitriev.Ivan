using LoyalCode.Dmitriev.Ivan.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoyalCode.Dmitriev.Ivan.Infrustructure.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetById(int id, CancellationToken cancellationToken = default);
        Task<User?> GetByName(string name, CancellationToken cancellationToken = default);
        Task<bool> Exists(string name, CancellationToken cancellationToken = default);
        Task<User> Add(User user, CancellationToken cancellationToken = default);
    }

    internal class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetById(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .Include(u => u.Favorites)
                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        }

        public async Task<User?> GetByName(string name, CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .Include(u => u.Favorites)
                .FirstOrDefaultAsync(u => u.Name == name, cancellationToken);
        }

        public async Task<bool> Exists(string name, CancellationToken cancellationToken = default)
        {
            return await _context.Users.AnyAsync(u => u.Name == name, cancellationToken);
        }

        public async Task<User> Add(User user, CancellationToken cancellationToken = default)
        {
            await _context.Users.AddAsync(user, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return user;
        }
    }
}
