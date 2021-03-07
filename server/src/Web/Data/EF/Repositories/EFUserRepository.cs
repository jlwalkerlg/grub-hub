using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Web.Domain;
using Web.Domain.Users;
using Web.Features.Users;

namespace Web.Data.EF.Repositories
{
    public class EFUserRepository : IUserRepository
    {
        private readonly AppDbContext context;

        public EFUserRepository(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<User> GetByEmail(string email)
        {
            return await context.Users
                .SingleOrDefaultAsync(x => x.Email == new Email(email));
        }

        public async Task<User> GetById(UserId id)
        {
            return await context.Users.FindAsync(id);
        }

        public async Task<bool> EmailExists(string email)
        {
            var count = await context.Users
                .Where(x => x.Email == new Email(email))
                .CountAsync();

            return count > 0;
        }

        public async Task Add(User user)
        {
            await context.Users.AddAsync(user);
        }

        public async Task<Customer> GetCustomerById(UserId id)
        {
            return await context.Customers.FindAsync(id);
        }
    }
}
