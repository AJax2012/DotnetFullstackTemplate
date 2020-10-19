using SourceName.Data.GenericRepositories;
using SourceName.Data.Model.Role;
using System.Collections.Generic;

namespace SourceName.Data.Roles
{
    public interface IRoleRepository : IRepository<RoleEntity>, IIntegerRepository<RoleEntity>
    {
        int GetRoleCount();
        void InsertRoles(IEnumerable<RoleEntity> role);
    }
}
