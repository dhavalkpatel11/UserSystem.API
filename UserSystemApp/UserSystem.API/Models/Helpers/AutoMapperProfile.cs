using AutoMapper;
using UserSystem.API.Models.Entities.Users;
using UserSystem.API.Models.Request.Users;
using UserSystem.API.Models.Response.Security;
using UserSystem.API.Models.Response.Users;

namespace WebApi.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {            
            // AddUserRequest -> User
            CreateMap<User, LoginViewModel>();

            // AddUserRequest -> User
            CreateMap<AddUserRequest, User>();

            // UpdateRequest -> User
            CreateMap<UpdateUserRequest, User>();

            // User -> UserViewModel
            CreateMap<User, UserViewModel>();
        }
    }
}