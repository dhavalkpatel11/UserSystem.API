using UserSystem.API.Models.Entities.Users;
using UserSystem.API.Models.Request.Users;

namespace UserSystem.API.Interfaces.Services.Users
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAll();
        Task<User> Get(int userId);
        Task<User> Get(string email);
        Task Add(AddUserRequest request);
        Task Update(int userId, UpdateUserRequest request);
        Task Delete(int userId);
    }
}