using AutoMapper;
using UserSystem.API.Interfaces.Repositories.Users;
using UserSystem.API.Interfaces.Services.Users;
using UserSystem.API.Models.Entities.Users;
using UserSystem.API.Models.Helpers;
using UserSystem.API.Models.Request.Users;
using UserSystem.API.Infrastructure.Security;
namespace UserSystem.API.Services.Users
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;
        private readonly IMapper _mapper;
        private const string strongPasswordRules = @"
            - Has minimum 8 characters in length. Adjust it by modifying {8,}
            - At least one uppercase English letter.
            - At least one lowercase English letter.
            - At least one digit.
            - At least one special character.";
        public UserService(IUserRepository userRepo, IMapper mapper)
        {
            _userRepo = userRepo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            return await _userRepo.GetAll();
        }
        public async Task<User> Get(int userId)
        {
            var user = await _userRepo.Get(userId);
            if (user == null) throw new CustomAppException("User not found");
            return user;
        }
        public async Task<User> Get(string email)
        {
            var user = await _userRepo.Get(email);
            if (user == null) throw new CustomAppException("User not found");
            return user;
        }
        public async Task Add(AddUserRequest request)
        {
            var isExists = await _userRepo.CheckIfEmailAlreadyExists(request.Email);
            if (isExists) throw new CustomAppException("This email '" + request.Email + "' is already taken.");

            if(!PasswordValidator.IsPasswordStrong(request.Password))
                throw new CustomAppException($"Please enter strong password as per below rules: \n {strongPasswordRules}");

            // map model to user object
            var user = _mapper.Map<User>(request);

            // hash password
            user.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);

            // add new user
            await _userRepo.AddOrUpdateUser(user);
        }
        public async Task Update(int userId, UpdateUserRequest request)
        {
            var user = await Get(userId);
            if (user.Email != request.Email && (await _userRepo.CheckIfEmailAlreadyExists(request.Email, user.Id))) 
                throw new CustomAppException("This email '" + request.Email + "' is already taken.");

            string password = user.Password;
            
            // map model to user object
            _mapper.Map(request, user);

            // hash password if it was entered
            if (!string.IsNullOrWhiteSpace(request.Password))
            {
                if (!PasswordValidator.IsPasswordStrong(request.Password))
                    throw new CustomAppException($"Please enter strong password as per below rules: \n {strongPasswordRules}");

                password = BCrypt.Net.BCrypt.HashPassword(request.Password);
            }
            user.Password = password;

            // update user
            await _userRepo.AddOrUpdateUser(user);
        }
        public async Task Delete(int userId)
        {
            var user = await Get(userId);            
            user.DeletedDate = DateTime.UtcNow;

            // update user
            await _userRepo.AddOrUpdateUser(user);
        }

    }
}