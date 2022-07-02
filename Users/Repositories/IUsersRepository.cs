using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Users.Entities;

namespace Users.Repositories
{
    public interface IUsersRepository
    {
        Task<User> GetUserAsync(string email);
        Task<User> GetUserAsync(Guid id);
        Task<IEnumerable<User>> GetUsersAsync();
        Task CreateUserAsync(User user);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(Guid id);
    }
}