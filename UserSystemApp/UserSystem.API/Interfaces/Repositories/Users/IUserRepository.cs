using UserSystem.API.Models.Entities.Users;

namespace UserSystem.API.Interfaces.Repositories.Users
{
    public interface IUserRepository
    {
        Task<bool> CheckIfEmailAlreadyExists(string email, int? ignoreUserId = null);
        Task<User> Get(int userId);
        Task<User> Get(string email);
        Task<IEnumerable<User>> GetAll();
        Task AddOrUpdateUser(User user);
        
        //DeleteUser()
    }
}