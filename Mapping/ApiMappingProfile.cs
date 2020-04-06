using System.Linq;
using AutoMapper;
using SourceName.Api.Model.Roles;
using SourceName.Api.Model.User;
using SourceName.Service.Model.Roles;
using SourceName.Service.Model.Users;

namespace SourceName.Mapping
{
    public class ApiMappingProfile : Profile
    {
        public ApiMappingProfile()
        {
            // API => Service
            CreateMap<AuthenticateUserRequest, User>();

            CreateMap<CreateUserRequest, User>()
                .ForMember(
                    destination => destination.Roles,
                    opt => opt.MapFrom(source =>
                        source.RoleIds.Select(r => 
                            new UserRole
                            {
                                RoleId = r
                            })));

            CreateMap<CreateUserRequest, User>()
                .ForMember(
                    destination => destination.Roles,
                    opt => opt.MapFrom(source =>
                        source.RoleIds.Select(r =>
                            new UserRole
                            {
                                RoleId = r
                            })));

            // Service => Api
            CreateMap<User, UserResource>();
            CreateMap<UserCapabilities, UserCapabilitiesResource>();
            CreateMap<UserRole, UserRoleResource>();
            CreateMap<Role, RoleResource>();
        }
    }
}