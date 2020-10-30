using System.Linq;
using AutoMapper;
using SourceName.Api.Model;
using SourceName.Api.Model.Roles;
using SourceName.Api.Model.User;
using SourceName.Service.Model;
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
                    opt => opt.MapFrom(
                        source => source.RoleIds.Select(r => 
                            new UserRole
                            {
                                RoleId = r
                            })));

            CreateMap<UpdateUserRequest, User>()
                .ForMember(
                    destination => destination.Id,
                    opt => opt.Ignore()
                );

            // Service => Api
            CreateMap<User, UserResource>();
            CreateMap<UserCapabilities, UserCapabilitiesResource>();

            CreateMap<UserRole, RoleResource>()
                .ForMember(
                    destination => destination.Id,
                    opt => opt.MapFrom(
                        source => source.RoleId))
                .ForMember(
                    destination => destination.Name,
                    opt => opt.MapFrom(
                        source => source.Role.Name))
                .ForMember(
                    destination => destination.Description,
                    opt => opt.MapFrom(
                        source => source.Role.Description));

            CreateMap<Role, RoleResource>();
            CreateMap(typeof(SearchResult<>), typeof(SearchResultResource<>));
        }
    }
}