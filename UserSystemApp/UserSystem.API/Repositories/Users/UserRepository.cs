using Microsoft.EntityFrameworkCore;
using UserSystem.API.Data;
using UserSystem.API.Interfaces.Repositories.Users;
using UserSystem.API.Models.Entities.Users;
namespace UserSystem.API.Repositories.Users
{
    public class UserRepository : IUserRepository
    {
        private UserSystemDBContext _ctx;
        public UserRepository(UserSystemDBContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<bool> CheckIfEmailAlreadyExists(string email, int? ignoreUserId = null)
        {
            return await _ctx.Users.AnyAsync(x => x.Email.ToLower().Equals(email.ToLower()) 
            && (!ignoreUserId.HasValue || x.Id != ignoreUserId));
        }
        public async Task<User> Get(int userId)
        {
            return await _ctx.Users.Where(x => x.Id.Equals(userId)).SingleOrDefaultAsync();
        }
        public async Task<User> Get(string email)
        {
            return await _ctx.Users.Where(x => x.Email.ToLower().Equals(email.ToLower())).SingleOrDefaultAsync();
        }
        public async Task<IEnumerable<User>> GetAll()
        {
            return await _ctx.Users.ToListAsync();
        }
        public async Task AddOrUpdateUser(User user)
        {
            if (user.Id == 0)
            {
                await _ctx.Users.AddAsync(user);
            }
            else
            {
                _ctx.Users.Attach(user);
                _ctx.Entry(user).State = EntityState.Modified;
            }
            _ctx.SaveChanges();
        }
    }
}