using SourceName.Data.Roles;
using SourceName.Data.Implementation.GenericRepositories;
using SourceName.Data.Model.Role;
using SourceName.Data.Model;
using Microsoft.EntityFrameworkCore;

namespace SourceName.Data.Implementation.Role
{
    public class RoleRepository : IntegerRepositoryBase<RoleEntity>, IRoleRepository
    {
        public RoleRepository(EntityContext context) : base(context) { }
    }
}
