using SourceName.Data.GenericRepositories;
using SourceName.Data.Model.Role;

namespace SourceName.Data.Roles
{
    public interface IRoleRepository : IRepository<RoleEntity>, IIntegerRepository<RoleEntity>
    {
    }
}
