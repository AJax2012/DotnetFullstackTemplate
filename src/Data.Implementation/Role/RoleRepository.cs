using SourceName.Data.Roles;
using SourceName.Data.Implementation.GenericRepositories;
using SourceName.Data.Model.Role;
using SourceName.Data.Model;
using System.Linq;
using System.Collections.Generic;

namespace SourceName.Data.Implementation.Role
{
    public class RoleRepository : IntegerRepositoryBase<RoleEntity>, IRoleRepository
    {
        public RoleRepository(EntityContext context) : base(context) { }

        public int GetRoleCount()
        {
            return _context.Set<RoleEntity>().Count();
        }

        public void InsertRoles(IEnumerable<RoleEntity> roles)
        {
            _context.Set<RoleEntity>().AddRange(roles);
            _context.SaveChanges();
        }
    }
}
