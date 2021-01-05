using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Web.Features.Users;
using Web.Domain.Users;

namespace ApplicationTests.Users
{
    public class UserRepositorySpy : IUserRepository
    {
        public List<User> Users { get; } = new();

        public Task<User> GetByEmail(string email)
        {
            return Task.FromResult(Users.FirstOrDefault(x => x.Email.Address == email));
        }

        public Task Add(User user)
        {
            Users.Add(user);
            return Task.CompletedTask;
        }

        public Task<User> GetById(UserId id)
        {
            return Task.FromResult(Users.FirstOrDefault(x => x.Id == id));
        }

        public async Task<bool> EmailExists(string email)
        {
            return await GetByEmail(email) == null;
        }
    }
}
