using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Timesheets.Models;

namespace Timesheets.Data.Interfaces
{
    public interface IUserRepo
    {
        Task<User> GetItem(Guid id);
        Task<IEnumerable<User>> GetItems();
        Task Add(User item);
        Task Update(User item);
        
        
        Task<User> GetByLoginAndPasswordHash(string login, byte[] passwordHash);
        Task CreateUser(User user);
    }
}