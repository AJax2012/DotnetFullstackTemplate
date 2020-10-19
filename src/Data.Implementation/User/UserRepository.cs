using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SourceName.Data.Implementation.GenericRepositories;
using SourceName.Data.Model;
using SourceName.Data.Model.Role;
using SourceName.Data.Model.User;
using SourceName.Data.Users;

namespace SourceName.Data.Implementation.User
{
    public class UserRepository : IntegerRepositoryBase<UserEntity>, IUserRepository
    {
        public UserRepository(EntityContext context) : base(context) {}

        public override async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.Set<UserEntity>().FindAsync(id);

            if (entity == null)
            {
                return false;
            }

            entity.IsActive = false;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<UserEntity> GetByIdWithRolesAsync(int id)
        {
            return await _context.Users
                .Include(u => u.Roles)
                .Include("Roles.Role")
                .SingleOrDefaultAsync(user => user.Id == id && user.IsActive == true);
        }

        public async Task<UserEntity> GetByUsernameWithRolesAsync(string username)
        {
            return await _context.Users
                .Include(u => u.Roles)
                .Include("Roles.Role")
                .SingleOrDefaultAsync(user => user.Username == username && user.IsActive == true);
        }

        public override async Task<UserEntity> UpdateAsync(UserEntity user)
        {
            var userEntity = await _context.Set<UserEntity>()
                .Include(u => u.Roles)
                .SingleOrDefaultAsync(u => u.Id == user.Id);

            userEntity.FirstName = user.FirstName;
            userEntity.LastName = user.LastName;
            userEntity.Username = user.Username;

            await _context.SaveChangesAsync();
            return userEntity;
        }

        public async Task AddUserRolesAsync(IEnumerable<UserRoleEntity> rolesToAdd)
        {
            //var rolesToRemove = _context.Set<UserRoleEntity>()
            //    .Where(ur => ur.UserId == userId && roleIdsToRemove.Contains(ur.RoleId));

            //if (rolesToRemove.Any())
            //{
            //    _context.RemoveRange(rolesToRemove);
            //}

            //var rolesToAdd = roleIdsToAdd.Select(r => new UserRoleEntity
            //{
            //    UserId = userId,
            //    RoleId = r
            //});

            if (rolesToAdd.Any())
            {
                await _context.Set<UserRoleEntity>().AddRangeAsync(rolesToAdd);
            }

            await _context.SaveChangesAsync();
        }

        public async Task RemoveUserRolesAsync(IEnumerable<UserRoleEntity> rolesToRemove)
        {
            if (rolesToRemove.Any())
            {
                _context.Set<UserRoleEntity>().RemoveRange(rolesToRemove);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<UserEntity> UpdatePasswordAsync(int? id, byte[] passwordHash, byte[] passwordSalt)
        {
            var userEntity = await _context.Users.SingleOrDefaultAsync(user => user.Id == id);

            if (userEntity == null)
            {
                return null;
            }

            userEntity.PasswordHash = passwordHash;
            userEntity.PasswordSalt = passwordSalt;

            await _context.SaveChangesAsync();
            return userEntity;
        }
    }
}