using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SourceName.Data.Implementation.GenericRepositories;
using SourceName.Data.Model;
using SourceName.Data.Model.Role;
using SourceName.Data.Model.User;
using SourceName.Data.Users;

namespace SourceName.Data.Implementation.User
{
    public class UserRepository : GuidRepositoryBase<UserEntity>, IUserRepository
    {
        public UserRepository(EntityContext context) : base(context) {}

        public override void Delete(Guid id)
        {
            var entity = _context.Set<UserEntity>().Single(u => u.Id == id);
            entity.IsActive = false;
            _context.SaveChanges();
        }

        public override UserEntity GetById(Guid id)
        {
            return _context.Set<UserEntity>()
                .Include(u => u.Roles)
                .Include("Roles.Role")
                .SingleOrDefault();
        }

        public UserEntity GetByUsernameWithRoles(string username)
        {
            return _context.Users
                .Include(u => u.Roles)
                .Include("Roles.Role")
                .SingleOrDefault(user => user.Username == username);
        }

        public override UserEntity Update(UserEntity inputUser)
        {
            var userEntity = _context.Set<UserEntity>()
                .Include(u => u.Roles)
                .Include("Roles.Role")
                .Single(u => u.Id == inputUser.Id);

            var rolesToRemove = userEntity.Roles.Where(
                existingRole =>
                    !inputUser.Roles.Any(role => existingRole.RoleId == role.RoleId));
            foreach (var roleToRemove in rolesToRemove)
            {
                _context.Set<UserRoleEntity>().Remove(roleToRemove);
            }

            var rolesToAdd = inputUser.Roles.Where(
                newRole =>
                    !userEntity.Roles.Any(existingRole => existingRole.RoleId == newRole.RoleId));
            foreach (var roleToAdd in rolesToAdd)
            {
                userEntity.Roles.Add(new UserRoleEntity
                {
                    UserId = inputUser.Id,
                    RoleId = roleToAdd.RoleId
                });
            }

            userEntity.FirstName = inputUser.FirstName;
            userEntity.LastName = inputUser.LastName;
            userEntity.Username = inputUser.Username;

            _context.SaveChanges();
            return userEntity;
        }

        public UserEntity UpdatePassword(Guid? id, byte[] passwordHash, byte[] passwordSalt)
        {
            var userEntity = _context.Users.Single(user => user.Id == id);

            userEntity.PasswordHash = passwordHash;
            userEntity.PasswordSalt = passwordSalt;

            _context.SaveChanges();
            return userEntity;
        }
    }
}