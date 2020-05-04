using AutoMapper;
using SourceName.Data.Model.Role;
using SourceName.Data.Model.User;
using SourceName.Service.Model.Roles;
using SourceName.Service.Model.Users;

namespace SourceName.Mapping
{
    public class ServiceMappingProfile : Profile
    {
        public ServiceMappingProfile()
        {
            // Service -> Data

            // Data -> Service

            // Two way mappings
            CreateMap<RoleEntity, Role>().ReverseMap();
            CreateMap<UserEntity, User>().ReverseMap();
            CreateMap<UserRoleEntity, UserRole>().ReverseMap();
        }
    }
}